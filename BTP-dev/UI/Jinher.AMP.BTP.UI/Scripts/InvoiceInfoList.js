function initEvent(){
    $('#invoice-lists').on('click','.del',function(){
        $('#modal').removeClass('hide');
    });
    $('#modal').on('click','.confirm-submit',function(){
        $('#modal').addClass('hide');
    }).on('click','.confirm-cancel',function(){
        $('#modal').addClass('hide');
    });
}
$(document).ready(function(){
    initEvent();
});

