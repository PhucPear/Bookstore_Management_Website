function checkDangNhapADMIN() {
    // Lấy giá trị từ các trường input
    var email = document.getElementById('email').value;
    var matKhau = document.getElementById('matkhau').value;

    // Kiểm tra tính đúng của dữ liệu
    if (kiemTraEmail(email) && kiemTraMatKhau(matKhau)) {
        $.ajax({
            type: "POST",
            url: "/Admin/AdminNguoiDung/DangNhap",
            data: { email: email, matKhau: matKhau },
            dataType: "json",
            success: function (result) {
                if (result.success) {
                    location.href = result.message;
                } else {
                    hienThiThongBao(result.message, 'alert-danger');
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
}

function kiemTraEmail(email) {
    var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (email.trim() === "") {
        hienThiThongBao("Email không được để trống", 'alert-danger');
        return false;
    } else if (!regex.test(email)) {
        hienThiThongBao("Định dạng email không hợp lệ", 'alert-danger');
        return false;
    }
    return true;
}

function kiemTraMatKhau(matKhau) {
    if (matKhau.trim() === "") {
        hienThiThongBao("Mật khẩu không được để trống", 'alert-danger');
        return false;
    }
    return true;
}

function hienThiThongBao(message, className) {
    var alertDiv = document.getElementById('userAlert');
    alertDiv.innerHTML = message;
    alertDiv.className = 'alert ' + className;
    alertDiv.style.display = 'block';
}