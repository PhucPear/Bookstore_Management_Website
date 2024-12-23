$(document).ready(function () {
    $('#huy').on('click', function () {
        var maDH = $(this).data('id');
        // Gán giá trị vào input ẩn trong modal
        $('#maHuy').val(maDH);
    });
});

$(document).ready(function () {
    $('#nhan').on('click', function () {
        var maDH = $(this).data('id');
        // Gán giá trị vào input ẩn trong modal
        $('#maNhan').val(maDH);
    });
});