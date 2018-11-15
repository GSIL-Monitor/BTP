/**
 * 入口
 */
$(document).ready(function(){
    //获取昵称 ，将昵称显示到输入框红
    //测试数据
    var name = "大头儿子";
    $('#editor-input').val(name);
    initEvent();
});
/**
 * 初始化事件
 */
function initEvent(){
    /**
     * 清空输入框
     */
    $('.editor-wrap').on('click','.fa-times',function(){
        $('#editor-input').val('');
    });
    /**
     * 确定按钮事件绑定
     */
    $('.bar-nav').on('click','.btn-link',function(){
        var value = $.trim($('#editor-input').val());
        if(value){
            //校验 提交修改内容
            toast('修改成功');
        }else{
            toast('请输入昵称')
        }
    });
}