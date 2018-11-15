$(function () {
    $('.check-box').on('click', function () {
        var hasClass = $(this).toggleClass('active').hasClass('active');
        var setUp = $(this).closest('.set-up');
        if (hasClass) {
            $('.set-up').removeClass('active');
            setUp.addClass('active');
            toast('选择成功');
        } else {
            setUp.removeClass('active');
        }
    })
    $('.delete').on('click', function () {
        $('.sure-delete').removeClass('hide').attr('data-li', $(this).closest('li').index());
    })
    /*弹出框*/
    $('.cancel').on('click', function () {
        $(this).closest('.sure-delete').addClass('hide');
    })
    $('.sure').on('click', function () {
        var liIndex = $(this).closest('.sure-delete').attr('data-li');
        $('.receiving-information').eq(liIndex).remove();
        $(this).closest('.sure-delete').addClass('hide');
    })
});