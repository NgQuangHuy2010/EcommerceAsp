
$(document).ready(function () {
    $('#province').change(function () {
        var selectedProvince = $(this).val();
        $('#selected_province').val(selectedProvince); // Cập nhật giá trị cho input hidden
        $('#district').html('<option value="">Chọn quận/huyện</option>');
        $('#ward').html('<option value="">Chọn phường/xã</option>');

        $.each(locations, function (key, value) {
            if (value.name === selectedProvince) {
                $.each(value.districts, function (key, district) {
                    $('#district').append('<option value="' + district.name + '">' + district.name + '</option>');
                });
            }
        });
    });

    $('#district').change(function () {
        var selectedDistrict = $(this).val();
        $('#selected_district').val(selectedDistrict); // Cập nhật giá trị cho input hidden
        $('#ward').html('<option value="">Chọn phường/xã</option>');

        $.each(locations, function (key, value) {
            $.each(value.districts, function (key, district) {
                if (district.name === selectedDistrict) {
                    $.each(district.wards, function (key, ward) {
                        $('#ward').append('<option value="' + ward.name + '">' + ward.name + '</option>');
                    });
                }
            });
        });
    });

    $('#ward').change(function () {
        var selectedWard = $(this).val();
        $('#selected_ward').val(selectedWard); // Cập nhật giá trị cho input hidden
    });
    //lấy id cập nhật từ form cập nhật qua submit
    $('#shipment-form').submit(function () {
        $('#selected_province').val($('#province').val());
        $('#selected_district').val($('#district').val());
        $('#selected_ward').val($('#ward').val());
    });
});
