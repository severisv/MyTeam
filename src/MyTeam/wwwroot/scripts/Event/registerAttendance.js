

$('.register-attendance-item').click(function () {
    var element = $(this);
    var attendanceId = element.data('attendanceId');
    var value = element.val();
    console.log(value);
    $.ajax({
        type: 'POST',
     url: Routes.CONFIRM_ATTENDANCE,
        data: {
            attendanceId: attendanceId,
            didAttend: value
        },
        success: function (response) {
          
        }
    });
});


