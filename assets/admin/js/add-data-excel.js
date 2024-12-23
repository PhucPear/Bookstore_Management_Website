$(document).ready(function () {
    $('#btnNhap').click(function () {
        const file = document.getElementById('fileExcel').files[0];
        if (file) {
            const formData = new FormData();
            formData.append('filePath', file);
            $.ajax({
                type: 'POST',
                url: '/Admin/AdminSanPham/ThemSanPhamBangFileExcel',
                data: formData,
                contentType: false,
                processData: false,
                success: function (result) {
                    if (result.success) {
                        hienThiThongBao(result.message, 'alert-success');
                    } else {
                        hienThiThongBao(result.message, 'alert-danger');
                    }
                },
                error: function (error) {
                    console.log(error);
                }
            });
        } else {
            hienThiThongBao('Vui lòng chọn file!!', 'alert-danger');
        }
    });
});

function hienThiThongBao(message, className) {
    var alertDiv = document.getElementById('fileAlert');
    alertDiv.innerHTML = message;
    alertDiv.className = 'alert ' + className;
    alertDiv.style.display = 'block';
}