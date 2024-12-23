function layTenFileAnh(inputId, labelId) {
    const hinhAnh = document.getElementById(inputId);
    const tenHinhAnh = hinhAnh.files[0].name;
    const tenAnhLabel = document.getElementById(labelId);
    tenAnhLabel.textContent = tenHinhAnh;
}


function luuFileAnh(inputId) {
    const fileHinh = document.getElementById(inputId).files[0];
    const tenHinhAnh = fileHinh.name;

    saveAs(fileHinh, tenHinhAnh);
}