//$(document).ready(function () {
//    var typingTimer;
//    var doneTypingInterval = 500; // Độ trễ sau khi nhập kết thúc (milliseconds)

//    // Sự kiện khi người dùng nhập vào ô tìm kiếm
//    $('#searchInput').on('keyup', function () {
//        clearTimeout(typingTimer);
//        if ($('#searchInput').val()) {
//            typingTimer = setTimeout(doneTyping, doneTypingInterval);
//        }
//    });

//    // Hàm gửi yêu cầu tìm kiếm
//    function doneTyping() {
//        var query = $('#searchInput').val();

//        // Gửi yêu cầu Ajax
//        $.ajax({
//            url: '/SanPham/HienThiGoiY',
//            method: 'POST',
//            data: { searchSP: query },
//            success: function (data) {
//                // Xử lý dữ liệu gợi ý
//                updateSuggestions(data);
//            },
//            error: function (error) {
//                console.error('Lỗi khi gửi yêu cầu tìm kiếm:', error);
//            }
//        });
//    }

//    // Hàm cập nhật gợi ý trên giao diện
//    function updateSuggestions(suggestions) {
//        // Xóa các gợi ý cũ
//        $('#searchResult').empty();

//        // Hiển thị gợi ý mới
//        suggestions.forEach(function (item) {
//            $('#searchResult').append('<li>' + item + '</li>');
//        });
//    }
//});

$(document).ready(function () {
    $('#searchInput').autocomplete({
        source: function (request, response) {
            $.ajax({
                url: '/SanPham/HienThiGoiY',
                type: 'GET',
                dataType: 'json',
                data: { search: request.term },
                success: function (data) {
                    response($.map(data, function (item) {
                        return { label: item.label, id: item.id };
                    }));
                }
            });
        },
        select: function (event, ui) {
            window.location.href = '/SanPham/ChiTietSanPham?maSP=' + ui.item.id;
        }
    });
});
