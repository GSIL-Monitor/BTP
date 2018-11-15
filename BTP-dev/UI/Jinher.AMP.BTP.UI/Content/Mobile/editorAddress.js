$(function () {
    //下拉框
    $('.select-content').mobiscroll().select({
        cancelText:'',
        setText: '完成',
        onSelect: function (valueText) {
            console.log(valueText);
        }
    });

    var textarea = $('.address').find('textarea');
    if (textarea.val() != '') {
        textarea.siblings('.label').addClass('hide');
    }
    textarea.on('input', function () {
        if ($(this).val() != '') {
            $(this).siblings('.label').addClass('hide');
        } else {
            $(this).siblings('.label').removeClass('hide');
        }
    })
    $('.default-address').find('.check-box').on('click', function () {
        $(this).closest('.default-address').toggleClass('active');
    })
});