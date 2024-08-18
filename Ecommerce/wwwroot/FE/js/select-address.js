$(document).ready(function () {

    var data = []; // Biến để lưu trữ dữ liệu JSON

    // Khi trang load, lấy danh sách các tỉnh/thành phố
    $.ajax({
        url: 'Checkout/GetLocations',
        type: 'GET',
        success: function (response) {
            data = response; // Lưu dữ liệu vào biến toàn cục

            // Xử lý dữ liệu JSON và điền vào select tỉnh/thành phố
            data.forEach(function (item) {
                $('#city').append($('<option>', {
                    value: item.code,
                    text: item.name
                }));
            });
        },
        error: function (xhr, status, error) {
            console.error("Lỗi khi lấy danh sách tỉnh/thành phố:", error);
        }
    });

    // Khi người dùng chọn tỉnh/thành phố
    $('#city').change(function () {
        var cityCode = $(this).val();
        $('#district').empty().append('<option value="">-- Chọn Quận/Huyện --</option>');
        $('#ward').empty().append('<option value="">-- Chọn Phường/Xã --</option>');

        if (cityCode) {
            var city = data.find(x => x.code == cityCode);
            if (city) {
                var districts = city.districts;
                districts.forEach(function (district) {
                    $('#district').append($('<option>', {
                        value: district.code,
                        text: district.name
                    }));
                });
            }
        }
    });

    // Khi người dùng chọn quận/huyện
    $('#district').change(function () {
        var cityCode = $('#city').val();
        var districtCode = $(this).val();
        $('#ward').empty().append('<option value="">-- Chọn Phường/Xã --</option>');

        if (cityCode && districtCode) {
            var city = data.find(x => x.code == cityCode);
            if (city) {
                var district = city.districts.find(d => d.code == districtCode);
                if (district) {
                    var wards = district.wards;
                    wards.forEach(function (ward) {
                        $('#ward').append($('<option>', {
                            value: ward.code,
                            text: ward.name
                        }));
                    });
                }
            }
        }
    });

    // Xử lý việc gửi form
    $('form').submit(function () {
        var addressComponents = [
            $('#city option:selected').text(),
            $('#district option:selected').text(),
            $('#ward option:selected').text()
        ];

        $('#AddressComponents').val(JSON.stringify(addressComponents)); 
    });
});
