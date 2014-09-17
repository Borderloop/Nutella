<?php

require_once('db_handler.php');
$conn = iniConn();

$query = "SELECT ws.id,ws.name,ws.url,ws.shipping_cost,JM.combinedmarks,JS.combinedsenders,JA.approved,JA.combinedaffiliates,JP.combinedmethods,JO.combinedoperatingcountries,JSC.combinedsendingcountries,JL.combinedlanguages
     FROM webshop ws
LEFT JOIN (SELECT MA.webshop_id,
                  GROUP_CONCAT(MA.mark) AS combinedmarks
             FROM mark MA 
         GROUP BY MA.webshop_id) JM ON JM.webshop_id = ws.id  
         
LEFT JOIN (SELECT SE.webshop_id,
                  GROUP_CONCAT(SE.sender) AS combinedsenders
             FROM sender SE 
         GROUP BY SE.webshop_id) JS ON JS.webshop_id = ws.id
         
LEFT JOIN (SELECT AF.webshop_id,AF.approved,
                  GROUP_CONCAT(AF.affiliate) AS combinedaffiliates
             FROM affiliate AF
         GROUP BY AF.webshop_id) JA ON JA.webshop_id = ws.id     
             
LEFT JOIN (SELECT PM.webshop_id,
                  GROUP_CONCAT(PM.method) AS combinedmethods
             FROM payment_method PM
         GROUP BY PM.webshop_id) JP ON JP.webshop_id = ws.id
         
LEFT JOIN (SELECT OC.webshop_id,
                  GROUP_CONCAT(OC.country) AS combinedoperatingcountries
             FROM operatingcountry OC
         GROUP BY OC.webshop_id) JO ON JO.webshop_id = ws.id
         
LEFT JOIN (SELECT SC.webshop_id,
                  GROUP_CONCAT(SC.country) AS combinedsendingcountries
             FROM sendingcountry SC
         GROUP BY SC.webshop_id) JSC ON JSC.webshop_id = ws.id
         
LEFT JOIN (SELECT LG.webshop_id,
                  GROUP_CONCAT(LG.language) AS combinedlanguages
             FROM language LG
         GROUP BY LG.webshop_id) JL ON JL.webshop_id = ws.id ";

$where = "WHERE ";

$data = loadTable($conn, $query);

if ($_SERVER["REQUEST_METHOD"]=="POST"){
	$data = loadTable($conn, $query);
	if (isset($_POST['dataRows'])){
		foreach($_POST['dataRows'] as $datarow){
			deleteRow($conn,$datarow);

		}
	}
	if (isset($_POST['languageSelection']) && !empty($_POST['languageSelection'])){
		$modifier = "";
		foreach($_POST['languageSelection'] as $language){
			$where .= "$modifier JL.combinedlanguages LIKE '%$language%'";
			$modifier = "OR";
		}
		$data = loadTable($conn,($query.$where));
	}

	/*if (isset($_POST['searchName'])){
		$name = $_POST['searchName'];
		$modifier = "";
		$where .= "$modifier ws.name LIKE '%$name%'";
		$modifier = "OR";
		$data = loadTable($conn,($query.$where));
	}*/
}

//echo $data;


function deleteRow($conn, $rowId){
	$stmt = $conn->prepare("DELETE FROM webshop WHERE id=:wsId");
	$stmt->execute(array(
		"wsId" => $rowId
		));
	echo "Deleted row (ID:".$rowId.")<br/>";
}

function loadTable ($conn, $query){
	$stmt = $conn->prepare($query);
	$stmt->execute();
	$data = "";
	$data.="<table><tr>";
	$data.="<th>ID</th>";
	$data.="<th></th>";
	$data.="<th></th>";
	$data.="<th>Naam</th>";
	$data.="<th>Url</th>";
	$data.="<th>Affiliates</th>";
	$data.="<th>Languages</th>";
	$data.="<th>Levert naar</th>";
	$data.="</tr><form method='post' action=''>";

	$rowCounter = 0;
	while($row = $stmt->fetch()){
		$rowId = $row['id'];
		if($rowCounter%2 == 0){
			$rowStyle = "even";
		}
		else{
			$rowStyle = "uneven";
		}
		$data.="<tr class ='$rowStyle'>";
		$data.="<td>".$row['id']."</td>";
		$data.="<td><input type='checkbox' name='dataRows[]' value='$rowId'></td>";
		$data.="<td>".$row['approved']."</td>";
		$data.="<td>".$row['name']."</td>";
		$data.="<td>".$row['url']."</td>";
		$data.="<td>".$row['combinedaffiliates']."</td>";
		$data.="<td>".$row['combinedlanguages']."</td>";
		$data.="<td>".$row['combinedsendingcountries']."</td>";
		$data.="</tr>";
		$rowCounter++;
	}
	$data.="</table>
	<input class='contact-button' name='submit' type='submit' value='DELETE'>
	</form>
	<a class='contact-button' href='/addShop.php'>Add Shop</a>";
	return $data;
}
$conn = null;
?>

<link rel="stylesheet" href="css/stylesheetMAIN.css" type="text/css">
<head>
</head>
<body>
	<form method="post" action="">
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
		||
		<input class="approvedCheckbox" type="checkbox" name="approvedSelection[]" value="1">Approved</input>
		<input class="approvedCheckbox" type="checkbox" name="approvedSelection[]" value="2">Denied</input>
		<input class="approvedCheckbox" type="checkbox" name="approvedSelection[]" value="3">Pending</input>
		<input class="contact-button" name="submit" type="submit" value="Filter">
	</form>
	<div id="start_results"><?php //echo $data; ?></div>
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
		$(":checkbox").change(function(e){
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