<?php

function IniConn(){
	// Create connection
	$username = '';
	$pwd = '';
	$con= new PDO('mysql:host=149.210.175.211;dbname=borderloop;charset=utf8', $username, $pwd);
	$con->setAttribute(PDO::ATTR_EMULATE_PREPARES, false);
	$con->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
	return $con;
}

function closeConn($connLink){
	// Create connection
	mysqli_close($connLink);
}

?>