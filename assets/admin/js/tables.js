$(document).ready(function () {
    $('.delete').on('click', function () {
        // Lấy giá trị của thuộc tính data-id từ nút "Xoá"
        var ma = $(this).data('id');
        // Gán giá trị vào input ẩn trong modal
        $('#maXoa').val(ma);
    });
});

$(document).ready(function () {
    $('.edit').on('click', function () {
        var ma = $(this).data('id');
        $('#maSua').val(ma);
        // Gán mã vào biến Session
        $.ajax({
            url: "/Admin/Public/GanMaChoSession",
            type: "POST",
            data: { ma: ma},
            success: function (result) {
                console.log(result.message);
            },
            error: function (error) {
                console.log(error);
            }
        });
    });
});

$(document).ready(function () {
    $('.mo').on('click', function () {
        var ma = $(this).data('id');
        $('#maMo').val(ma);
    });
});

$(document).ready(function () {
    // Lắng nghe sự kiện thay đổi của dropdown
    $("#danhMuc").change(function () {
        // Lấy giá trị của dropdown và cập nhật giá trị của input hidden
        $("#ma").val($(this).val());
    });
});


