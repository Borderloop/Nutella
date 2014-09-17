<?php

require_once('db_handler.php');
$conn = iniConn();


function deleteRow($conn, $rowId){
	$stmt = $conn->prepare("DELETE FROM webshop WHERE id=:wsId");
	$stmt->execute(array(
		"wsId" => $rowId
		));
	echo "Deleted row (ID:".$rowId.")<br/>";
}

$conn = null;
?>

<link rel="stylesheet" href="css/stylesheetMAIN.css" type="text/css">
<head>
</head>
<body>
	<form method="post" action="">
		<b>Taal:</b>
		<input class="languageCheckbox" type="checkbox" name="languageSelection[]" value="nl">NL</input>
		<input class="languageCheckbox" type="checkbox" name="languageSelection[]" value="be">BE</input>
		<input class="languageCheckbox" type="checkbox" name="languageSelection[]" value="de">DE</input>
		<input class="languageCheckbox" type="checkbox" name="languageSelection[]" value="en">EN</input>
		<input class="languageCheckbox" type="checkbox" name="languageSelection[]" value="fr">FR</input>
		<input class="languageCheckbox" type="checkbox" name="languageSelection[]" value="it">IT</input>
		<input class="languageCheckbox" type="checkbox" name="languageSelection[]" value="es">ES</input>
		<input class="languageCheckbox" type="checkbox" name="languageSelection[]" value="pt">PT</input>
		<input class="languageCheckbox" type="checkbox" name="languageSelection[]" value="se">SE</input>
		<input class="languageCheckbox" type="checkbox" name="languageSelection[]" value="no">NO</input>
		<input class="languageCheckbox" type="checkbox" name="languageSelection[]" value="dk">DK</input>
		<input class="languageCheckbox" type="checkbox" name="languageSelection[]" value="pl">PL</input>
		<input class="languageCheckbox" type="checkbox" name="languageSelection[]" value="ie">IE</input>
		<input class="languageCheckbox" type="checkbox" name="languageSelection[]" value="fi">FI</input>
		| <b>Approved:</b>
		<input class="approvedCheckbox" type="checkbox" name="approvedSelection[]" value="1">Approved</input>
		<input class="approvedCheckbox" type="checkbox" name="approvedSelection[]" value="2">Pending</input>
		<input class="approvedCheckbox" type="checkbox" name="approvedSelection[]" value="3">Denied</input>
	</form>

	<div id="search_results"></div>
	<form method="post" action="">
		<input id="search_term" type="text" name="search_term" placeholder="Zoek op naam...">
		<input id="search_button" name="submit" type="submit" value="Zoek">
	</form>

	<script src="jquery/jquery-1.11.1.min.js"></script>
	<script type="text/javascript">
	$(document).ready(function(){
	ajax_search();
		$("#search_button").click(function(e){
			e.preventDefault();
			ajax_search();
		});
		$("#search_term").keyup(function(e){
			e.preventDefault();
			ajax_search();
		});
		$(":checkbox").change(function(){
			ajax_search();
		});
	});

	function ajax_search(){
		$("#search_results").show();
		var searchValue = $("#search_term").val();
		var languageArr=[];
		var approvedArr=[];
		$('.languageCheckbox:checked').each(function(){
			languageArr.push( $(this).val());
		})
		$('.approvedCheckbox:checked').each(function(){
			approvedArr.push( $(this).val());
		})
		$.post("/searchWebshop.php", {search_term : searchValue, filter_language : languageArr, filter_approved : approvedArr}, function(data){
			if(data.length>0){
				$("#start_results").slideUp();
				$("#search_results").html(data);
			}
		})
	}

	</script>
</body>