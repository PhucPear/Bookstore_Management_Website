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


$(document).ready(function () {
    $("#btnOTP").click(function () {
        var email = $("#email").val();
        if (kiemTraEmail(email)) {
            $.ajax({
                type: "POST",
                url: "/KhachHang/GuiMaOTP",
                data: { email: email },
                dataType: "json",
                success: function (result) {
                    if (result.success) {
                        hienThiThongBao(result.message, 'alert-success');
                        $("#otpGroup").show();
                        $("#matKhauGroup").show();
                        $("#btnGroup").show();
                    } else {
                        hienThiThongBao(result.message, 'alert-danger');
                    }
                },
                error: function (error) {
                    console.log(error);
                }
            });
        }
    });
});

function checkQuenMatKhau() {
    // Lấy giá trị từ các trường input
    var email = document.getElementById('email').value;
    var otp = document.getElementById('maOTP').value;
    var matKhau = document.getElementById('matkhau').value;

    // Kiểm tra tính đúng của dữ liệu
    if (kiemTraEmail(email) && kiemTraOTP(otp) && kiemTraMatKhau(matKhau)) {
        $.ajax({
            type: "POST",
            url: "/KhachHang/QuenMatKhau",
            data: { email: email, otp: otp, matKhau: matKhau },
            dataType: "json",
            success: function (result) {
                if (result.success) {
                    $("#email").val("");
                    $("#maOTP").val("");
                    $("#matkhau").val("");
                    hienThiThongBao(result.message, 'alert-success');
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

function checkDangNhap() {
    // Lấy giá trị từ các trường input
    var email = document.getElementById('email').value;
    var matKhau = document.getElementById('matkhau').value;

    // Kiểm tra tính đúng của dữ liệu
    if (kiemTraEmail(email) && kiemTraMatKhau(matKhau)) {
        $.ajax({
            type: "POST",
            url: "/KhachHang/DangNhap",
            data: { email: email, matKhau: matKhau },
            dataType: "json",
            success: function (result) {
                if (result.success) {
                    location.href = "/trang-chu";
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

function checkDangKy() {
    // Lấy giá trị từ các trường input
    var hoTen = document.getElementById('hoTen').value;
    var sdt = document.getElementById('sdt').value;
    var email = document.getElementById('email').value;
    var matKhau = document.getElementById('matkhau').value;
    var diaChi = document.getElementById('diaChi').value;
    var reMatKhau = document.getElementById('reMatKhau').value;


    // Kiểm tra tính đúng của dữ liệu
    if (kiemTraText(hoTen) && kiemTraEmail(email) && kiemTraSDT(sdt) && kiemTraText(diaChi) && kiemTraMatKhau(matKhau) && xacNhanMK(matKhau, reMatKhau)) {
        $.ajax({
            type: "POST",
            url: "/KhachHang/DangKy",
            data: { hoTen: hoTen, email: email, sdt: sdt, diaChi: diaChi, matKhau: matKhau },
            dataType: "json",
            success: function (result) {
                if (result.success) {
                    $("#hoTen").val("");
                    $("#sdt").val("");
                    $("#diaChi").val("");
                    $("#email").val("");
                    $("#matkhau").val("");
                    $("#reMatKhau").val("");
                    hienThiThongBao(result.message, 'alert-success');
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

function kiemTraOTP(otp) {
    if (otp.trim() === "") {
        hienThiThongBao("Vui lòng nhập mã xác nhận OTP", 'alert-danger');
        return false;
    } else if (isNaN(otp) || parseInt(otp) != otp) {
        hienThiThongBao("Mã xác nhận OTP phải là số nguyên", 'alert-danger');
        return false;
    }
    return true;
}

function kiemTraSDT(phone) {
    if (phone.trim() === "") {
        hienThiThongBao("Số điện thoại không được bỏ trống", 'alert-danger');
        return false;
    }

    var phoneRegex = /^(0\d{9})$/;
    var isValid = phoneRegex.test(phone);

    if (!isValid) {
        hienThiThongBao("Số điện thoại không hợp lệ", 'alert-danger');
    }

    return isValid;
}


function kiemTraText(text) {
    if (text.trim() === "") {
        hienThiThongBao("Thông tin không được bỏ trống", 'alert-danger');
        return false;
    }
    return true;
}

function xacNhanMK(mk, rmk) {
    if (rmk !== mk) {
        hienThiThongBao("Mật khẩu nhập lại không trùng khớp!!", 'alert-danger');
        return false;
    }
    return true;
}

function kiemTraMatKhau(matKhau) {
    if (matKhau.trim() === "") {
        hienThiThongBao("Mật khẩu không được để trống", 'alert-danger');
        return false;
    } else if (matKhau.length < 6) {
        hienThiThongBao("Mật khẩu phải chứa ít nhất 6 ký tự.", 'alert-danger');
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