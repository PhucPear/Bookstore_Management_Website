// Hàm nhấn nút Thêm Sách vào Giỏ Hàng
function themSPVaoGioHang(maSP) {
    $.ajax({
        url: '/GioHang/ThemGioHang',
        method: 'POST',
        data: { maSP: maSP },
        success: function (result) {
            if (result.success) {
                // Cập nhật số lượng trong giỏ hàng
                var soLuong = result.sl;
                $("#slCart").text(soLuong);

                console.log("Sản phẩm đã được thêm vào giỏ hàng.");
            } else {
                location.href = "/KhachHang/DangNhap";
            }
        },
        error: function () {
            console.error("Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng.");
        }
    });
}