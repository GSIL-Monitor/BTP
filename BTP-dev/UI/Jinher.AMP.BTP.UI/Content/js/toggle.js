/* ========================================================================
* Ratchet: toggles.js v2.0.2
* http://goratchet.com/components#toggles
* ========================================================================
Adapted from Brad Birdsall's swipe
* Copyright 2014 Connor Sears
* Licensed under MIT (https://github.com/twbs/ratchet/blob/master/LICENSE)
* ======================================================================== */

!(function () {
    'use strict';

    var start = {};
    var touchMove = false;
    var distanceX = false;
    var toggle = false;

    var findToggle = function (target) {
        var i;
        var toggles = document.querySelectorAll('.toggle');

        for (; target && target !== document; target = target.parentNode) {
            for (i = toggles.length; i--; ) {
                if (toggles[i] === target) {
                    return target;
                }
            }
        }
    };

    window.addEventListener('click', function (e) {
        e = e.originalEvent || e;
        //ÐÞ¸Ä
        var event = e;

        toggle = findToggle(e.target);

        if (!toggle) {
            return;
        }

        var handle = toggle.querySelector('.toggle-handle');
        var toggleWidth = toggle.clientWidth;
        var handleWidth = handle.clientWidth;
        var offset = toggle.classList.contains('active') ? (toggleWidth - handleWidth) : 0;

        start = { pageX: event.pageX - offset, pageY: event.pageY };
        touchMove = false;
    });

    window.addEventListener('touchmove', function (e) {
        e = e.originalEvent || e;

        if (e.touches.length > 1) {
            return; // Exit if a pinch
        }
        if (!toggle) {
            return;
        }

        var handle = toggle.querySelector('.toggle-handle');
        var current = e.touches[0];
        var toggleWidth = toggle.clientWidth;
        var handleWidth = handle.clientWidth;
        var offset = toggleWidth - handleWidth;

        touchMove = true;
        distanceX = current.pageX - start.pageX;

        if (Math.abs(distanceX) < Math.abs(current.pageY - start.pageY)) {
            return;
        }

        e.preventDefault();

        if (distanceX < 0) {
            return (handle.style.marginLeft = '0px');
        }
        if (distanceX > offset) {
            return (handle.style.marginLeft = offset + 'px');
        }

        handle.style.marginLeft = distanceX + 'px';

        toggle.classList[(distanceX > (toggleWidth / 2 - handleWidth / 2)) ? 'add' : 'remove']('active');
    });

    window.addEventListener('click', function (e) {
        if (!toggle) {
            return;
        }
        var handle = toggle.querySelector('.toggle-handle');
        var toggleWidth = toggle.clientWidth;
        var handleWidth = handle.clientWidth;
        var offset = (toggleWidth - handleWidth);
        var slideOn = (!touchMove && !toggle.classList.contains('active')) || (touchMove && (distanceX > (toggleWidth / 2 - handleWidth / 2)));

        if (slideOn) {
            handle.style.marginLeft = offset + 'px';
        } else {
            handle.style.marginLeft = '0px';
        }

        toggle.classList[slideOn ? 'add' : 'remove']('active');

        e = new CustomEvent('toggle', {
            detail: { isActive: slideOn },
            bubbles: true,
            cancelable: true
        });

        toggle.dispatchEvent(e);

        touchMove = false;
        toggle = false;
    });

} ());