$(document).ready(function () {
    $('#btnPhucHoi').click(function () {
        const file = document.getElementById('fileRestore').files[0];
        if (file) {
            const fileName = file.name;
            $.ajax({
                type: 'POST',
                url: '/Admin/Database/RestoreDatabase',
                data: { fileName: fileName },
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
    var alertDiv = document.getElementById('adminAlert');
    alertDiv.innerHTML = message;
    alertDiv.className = 'alert ' + className;
    alertDiv.style.display = 'block';
}

function layTenFile(inputId, labelId) {
    const hinhAnh = document.getElementById(inputId);
    const tenHinhAnh = hinhAnh.files[0].name;
    const tenAnhLabel = document.getElementById(labelId);
    tenAnhLabel.textContent = tenHinhAnh;
}