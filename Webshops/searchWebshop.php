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

$where = "WHERE(ws.name LIKE :wsSearch)";

$searchTerm = $_POST['search_term'];
$languageFilter = false;
$approvedFilter = false;

if(isset($_POST['filter_language']) && !empty($_POST['filter_language'])){
foreach($_POST['filter_language'] as $filterlanguage){
  $modifier = " AND(";
    if($languageFilter){
      $modifier = " OR ";
    }
  $where .= $modifier."JL.combinedlanguages LIKE '%$filterlanguage%'";
  $languageFilter = true;
  }
  $where .= ")";
}

if(isset($_POST['filter_approved']) && !empty($_POST['filter_approved'])){
foreach($_POST['filter_approved'] as $filterapproved){
  $modifier = " AND(";
    if($approvedFilter){
      $modifier = " OR ";
    }
  $where .= $modifier."JA.approved LIKE '%$filterapproved%'";
  $approvedFilter = true;
  }
  $where .= ")";
}

$stmt = $conn->prepare($query.$where);
$stmt->execute(array(
	"wsSearch" => "%".$searchTerm."%"
	));

$result = "";
$result.="<table><tr>";
$result.="<th>ID</th>";
$result.="<th></th>";
$result.="<th></th>";
$result.="<th>Naam</th>";
$result.="<th>Url</th>";
$result.="<th>Affiliates</th>";
$result.="<th>Languages</th>";
$result.="<th>Levert naar</th>";
$result.="</tr><form method='post' action=''>";

$rowCounter = 0;
while($row = $stmt->fetch()){
	$rowId = $row['id'];
	if($rowCounter%2 == 0){
		$rowStyle = "even";
	}
	else{
		$rowStyle = "uneven";
	}
	$result.="<tr class ='$rowStyle'>";
	$result.="<td width='3%'>".$row['id']."</td>";
	$result.="<td width='2%'><input type='checkbox' name='resultRows[]' value='$rowId'></td>";
	$result.="<td width='2%'>".$row['approved']."</td>";
	$result.="<td width='15%'>"."<a href='/webshop.php?WebshopID=$rowId'>".$row['name']."</a></td>";
	$result.="<td width='15%'>".$row['url']."</td>";
	$result.="<td width='20%'>".$row['combinedaffiliates']."</td>";
	$result.="<td width='15%'>".$row['combinedlanguages']."</td>";
	$result.="<td width='15%'>".$row['combinedsendingcountries']."</td>";
	$result.="</tr>";
	$rowCounter++;
}
$result.="</table>
<input class='contact-button' name='submit' type='submit' value='DELETE'>
</form>
<a class='contact-button' href='/addShop.php'>Add Shop</a>";

echo $result;

$conn = null;
?>