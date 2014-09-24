<link rel="stylesheet" href="css/stylesheetMAIN.css" type="text/css">
<head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" /></head>
<?php

require_once('db_handler.php');

if(isset($_GET['WebshopID'])){
	$conn = iniConn();
	$wsId = $_GET['WebshopID'];

	$query = "SELECT ws.id,ws.name,ws.url,ws.shipping_cost,JM.combinedmarks,JS.combinedsenders,JA.approved,JA.combinedaffiliates,JA.extra,JP.combinedmethods,JO.combinedoperatingcountries,JSC.combinedsendingcountries,JL.combinedlanguages
     FROM webshop ws
LEFT JOIN (SELECT MA.webshop_id,
                  GROUP_CONCAT(MA.mark) AS combinedmarks
             FROM mark MA 
         GROUP BY MA.webshop_id) JM ON JM.webshop_id = ws.id  
         
LEFT JOIN (SELECT SE.webshop_id,
                  GROUP_CONCAT(SE.sender) AS combinedsenders
             FROM sender SE 
         GROUP BY SE.webshop_id) JS ON JS.webshop_id = ws.id
         
LEFT JOIN (SELECT AF.webshop_id,AF.approved,AF.extra,
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
         GROUP BY LG.webshop_id) JL ON JL.webshop_id = ws.id 
	WHERE ws.id = :wsId";

	$stmt = $conn->prepare($query);
	$stmt->execute(array(
		"wsId" => $wsId
		));

	while($row = $stmt->fetch()){
		echo "
		<table width='50%'>
			<tr class='even'>
				<td class='tableType'>Naam:</td>
				<td class='tableData'>".$row['name']."</td>
				<td class='tableType'>Klantnummer:</td>
				<td class='tableData'>".$row['id']."</td>
			</tr>
			<tr class='uneven'>
				<td class='tableType'>Url:</td>
				<td class='tableData'>".$row['url']."</td>
				<td class='tableType'>Affiliate toestemming:</td>
				<td class='tableData'><img src='/Images/Icons/approved".$row['approved'].".png'></td>
			</tr>
			<tr class='even'>
				<td class='tableType'>Affiliates:</td>
				<td class='tableData'>".$row['combinedaffiliates']."</td>
				<td class='tableType'>Affiliate link:</td>
				<td class='tableData'>".$row['extra']."</td>
			</tr>
			<tr class='uneven'>
				<td class='tableType'>Verzendkosten:</td>
				<td class='tableData'>&euro;".$row['shipping_cost']."</td>
				<td class='tableType'>Betaalmethodes:</td>
				<td class='tableData'>".$row['combinedmethods']."</td>

			</tr>
			<tr class='even'>
				<td class='tableType'>Opereert in:</td>
				<td class='tableData'>".$row['combinedoperatingcountries']."</td>
				<td class='tableType'>Verzend naar:</td>
				<td class='tableData'>".$row['combinedsendingcountries']."</td>
			</tr>
			<tr class='uneven'>
				<td class='tableType'>Verzend bedrijven:</td>
				<td class='tableData'>".$row['combinedsenders']."</td>
				<td class='tableType'>Keurmerken:</td>
				<td class='tableData'>".$row['combinedmarks']."</td>
			</tr>
			
		</table>
		<br/>
		<span class='contact-button'><a href='/overzicht.php'>Overzicht</a></span>
		";	
	}
}

?>