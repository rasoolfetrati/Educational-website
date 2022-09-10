
function submitDeleteForm(url,id) {

    Swal.fire({
        title: 'آیا شما از حذف اطمینان دارید؟?',
        text: "تغییرات غیر قابل بازگشت خواهند بود!",
        icon: 'warning',
        cancelButtonText: 'لغو',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'بله, حذفش کن!',
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: url + id,
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                success: function (result) {
                    Swal.fire({
                        position: 'top-end',
                        icon: 'success',
                        title: result,
                        showConfirmButton: false,
                        timer: 1500
                    });
                    $("#removeTr-" + id).fadeOut(3000, function () { $(this).remove(); });
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'اوپس...',
                        text: 'مشکلی پیش اومد...!',
                    });
                }
            })
        }
    })
}

function submitComment(url,id) {

    Swal.fire({
        title: 'آیا شما از تایید اطمینان دارید؟?',
        text: "تغییرات غیر قابل بازگشت خواهند بود!",
        icon: 'warning',
        cancelButtonText: 'لغو',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'بله, تایید کن!',
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: url + id,
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                success: function (result) {
                    Swal.fire({
                        position: 'top-end',
                        icon: 'success',
                        title: result,
                        showConfirmButton: false,
                        timer: 1500
                    });
                    $("#removeTr-" + id).fadeOut(3000, function () { $(this).remove(); });
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'اوپس...',
                        text: 'مشکلی پیش اومد...!',
                    });
                }
            })
        }
    })
}

function submitDeletebasketForm(url, id) {

    Swal.fire({
        title: 'آیا شما از حذف اطمینان دارید؟?',
        text: "تغییرات غیر قابل بازگشت خواهند بود!",
        icon: 'warning',
        cancelButtonText: 'لغو',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'بله, حذفش کن!',
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: 'POST',
                url: url + id,
                contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                success: function (result) {
                    Swal.fire({
                        position: 'top-end',
                        icon: 'success',
                        title: result,
                        showConfirmButton: false,
                        timer: 1500
                    });
                    $("#removeTr-" + id).fadeOut(3000, function () { $(this).remove(); window.location.reload(); });
                    
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'اوپس...',
                        text: 'مشکلی پیش اومد...!',
                    });
                }
            })
        }
    })
}