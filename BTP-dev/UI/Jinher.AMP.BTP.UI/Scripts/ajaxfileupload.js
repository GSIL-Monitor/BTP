
jQuery.extend({


    createUploadIframe: function (id, uri) {
        //create frame
        var frameId = 'jUploadFrame' + id;

        if (navigator.appName == "Microsoft Internet Explorer" && navigator.appVersion .split(";")[1].replace(/[ ]/g,"") != "MSIE9.0"){
            var io = document.createElement('<iframe id="' + frameId + '" name="' + frameId + '" />');
        }
        else {
            var io = document.createElement('iframe');
            io.id = frameId;
            io.name = frameId;
        }
        io.style.position = 'absolute';
        io.style.top = '-1000px';
        io.style.left = '-1000px';

        document.body.appendChild(io);

        return io
    },
    createUploadForm: function (id, s) {
        //create form	
        var formId = 'jUploadForm' + id;
        var fileId = 'jUploadFile' + id;
        var form = $('<form  action="" method="POST" name="' + formId + '" id="' + formId + '" enctype="multipart/form-data"></form>');
        var oldElement = $('#' + s.fileElementId);
        var newElement = $(oldElement).clone(true);
        var nameInput = $('<input name="toGetTheUniqueKey_Input" type="hidden" value="' + s.uniqueKey + '" />');
        $(oldElement).attr('id', fileId);
        $(oldElement).before(newElement);
        $(oldElement).appendTo(form);
        //set attributes
        $(form).css('position', 'absolute');
        $(form).css('top', '-1200px');
        $(form).css('left', '-1200px');
        $(form).appendTo('body');
        nameInput.appendTo($(form));
        return form;
    },

    ajaxFileUpload: function (s) {
        // TODO introduce global settings, allowing the client to modify them for all requests, not only timeout		
        s = jQuery.extend({}, jQuery.ajaxSettings, s);
        var id = new Date().getTime()
        var form = jQuery.createUploadForm(id, s);
        var io = jQuery.createUploadIframe(id, s.secureuri);
        var frameId = 'jUploadFrame' + id;
        var formId = 'jUploadForm' + id;
        // Watch for a new set of requests
        if (s.global && !jQuery.active++) {
            jQuery.event.trigger("ajaxStart");
        }
        var requestDone = false;
        // Create the request object
        var xml = {}
        if (s.global)
            jQuery.event.trigger("ajaxSend", [xml, s]);
        // Wait for a response to come back
        var uploadCallback = function (isTimeout) {
            var io = document.getElementById(frameId);
            try {
                if (io.contentWindow) {
                    xml.responseText = io.contentWindow.document.body ? io.contentWindow.document.body.innerHTML : null;
                    xml.responseXML = io.contentWindow.document.XMLDocument ? io.contentWindow.document.XMLDocument : io.contentWindow.document;

                } else if (io.contentDocument) {
                    xml.responseText = io.contentDocument.document.body ? io.contentDocument.document.body.innerHTML : null;
                    xml.responseXML = io.contentDocument.document.XMLDocument ? io.contentDocument.document.XMLDocument : io.contentDocument.document;
                }
            } catch (e) {
                //handlerError只在jquery-1.4.2之前的版本中存在，jquery-1.6 和1.7中都没有这个函数了，因此在1.4.2中将这个函数复制到了ajaxFileUpload.js中，问题解决
                //jQuery.handleError(s, xml, null, e);
                //调用自身脚本内方法处理异常
                handleError(s, xml, null, e);
            }
            if (xml || isTimeout == "timeout") {
                requestDone = true;
                var status;
                try {
                    status = isTimeout != "timeout" ? "success" : "error";
                    // Make sure that the request was successful or notmodified
                    if (status != "error") {
                        // process the data (runs the xml through httpData regardless of callback)
                        if (xml.responseText == "") {
                            return;
                        }
                        var data = jQuery.uploadHttpData(xml, s.dataType);
                        // If a local callback was specified, fire it and pass it the data
                        if (s.success)
                            s.success(data, status);

                        // Fire the global callback
                        if (s.global)
                            jQuery.event.trigger("ajaxSuccess", [xml, s]);
                    } else
                        //handlerError只在jquery-1.4.2之前的版本中存在，jquery-1.6 和1.7中都没有这个函数了，因此在1.4.2中将这个函数复制到了ajaxFileUpload.js中，问题解决
                        //jQuery.handleError(s, xml, null, e);
                        //调用自身脚本内方法处理异常
                        handleError(s, xml, status, null);
                } catch (e) {
                    status = "error";
                    //handlerError只在jquery-1.4.2之前的版本中存在，jquery-1.6 和1.7中都没有这个函数了，因此在1.4.2中将这个函数复制到了ajaxFileUpload.js中，问题解决
                    //jQuery.handleError(s, xml, null, e);
                    //调用自身脚本内方法处理异常
                    handleError(s, xml, status, e);
                }

                // The request was completed
                if (s.global)
                    jQuery.event.trigger("ajaxComplete", [xml, s]);

                // Handle the global AJAX counter
                if (s.global && ! --jQuery.active)
                    jQuery.event.trigger("ajaxStop");

                // Process result
                if (s.complete)
                    s.complete(xml, status);

                jQuery(io).unbind()

                setTimeout(function () {
                    try {
                        $(io).remove();
                        $(form).remove();

                    } catch (e) {
                        //handlerError只在jquery-1.4.2之前的版本中存在，jquery-1.6 和1.7中都没有这个函数了，因此在1.4.2中将这个函数复制到了ajaxFileUpload.js中，问题解决
                        //jQuery.handleError(s, xml, null, e);
                        //调用自身脚本内方法处理异常
                        handleError(s, xml, null, e);
                    }

                }, 100)

                xml = null

            }
        }
        // Timeout checker
        if (s.timeout > 0) {
            setTimeout(function () {
                // Check to see if the request is still happening
                if (!requestDone) uploadCallback("timeout");
            }, s.timeout);
        }
        try {
            // var io = $('#' + frameId);
            var form = $('#' + formId);
            $(form).attr('action', s.url);
            $(form).attr('method', 'POST');
            $(form).attr('target', frameId);
            $(form).attr('target', frameId);
            $('#' + formId + '-nameInput').val(s.imageName);
            if (form.encoding) {
                form.encoding = 'multipart/form-data';
            }
            else {
                form.attr("enctype", 'multipart/form-data');
            }
            $("#" + frameId).load(function () {
                uploadCallback();
            });
            setTimeout(function () { $(form).submit(); }, 0);

        } catch (e) {
            jQuery.handleError(s, xml, null, e);
        }
        return { abort: function () { } };

    },

    uploadHttpData: function (r, type) {
        var data = !type;
        data = type == "xml" || data ? r.responseXML : r.responseText;
        // If the type is "script", eval it in global context
        if (type == "script")
            jQuery.globalEval(data);
        // Get the JavaScript object, if JSON is used.
        if (type == "json")
        //            eval("data = " + data);
            data = jQuery.parseJSON(data);
        // evaluate scripts within html
        if (type == "html")
            jQuery("<div>").html(data).evalScripts();
        //alert($('param', data).each(function(){alert($(this).attr('value'));}));
        return data;
    },
    //Added By Yangam 2012.12.12
    //handlerError只在jquery-1.4.2之前的版本中存在，jquery-1.6 和1.7中都没有这个函数了，因此在1.4.2中将这个函数复制到了ajaxFileUpload.js中，问题解决
    //添加出现异常处理的方法，handlerError只在jquery-1.4.2之前的版本中存在，jquery-1.6 和1.7中都没有这个函数了，因此在1.4.2中将这个函数复制到了ajaxFileUpload.js中
    handleError: function( s, xhr, status, e ) 		{
    // If a local callback was specified, fire it
		if ( s.error ) {
			s.error.call( s.context || s, xhr, status, e );
		}
		// Fire the global callback
		if ( s.global ) {
			(s.context ? jQuery(s.context) : jQuery.event).trigger( "ajaxError", [xhr, s, e] );
		}
	}
})

