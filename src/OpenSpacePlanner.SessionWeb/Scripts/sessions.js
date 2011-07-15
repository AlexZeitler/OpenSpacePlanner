function refresh() {

	$.ajax({
			type: "POST",
			url: "/Home/Samstag/",
			dataType: 'html',
			cache: false,
			success: function(result) {
				$('#samstag').empty();
				$("#samstag").append(result);
			},
			error: function(error, response) {
				alert(error.responseText);
			}
		});

	$.ajax({
			type: "POST",
			url: "/Home/Sonntag/",
			dataType: 'html',
			cache: false,
			success: function(result) {
				$('#sonntag').empty();
				$("#sonntag").append(result);
			},
			error: function(error, response) {
				alert(error.responseText);
			}
		});
}