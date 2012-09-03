<?php 
 error_reporting(E_ALL);
 ini_set("display_errors", 1);

print_r($_REQUEST);
print_r($_POST);

if(!isset($_REQUEST['assembly']) || !isset($_REQUEST['name']) || !isset($_REQUEST['error']) || !isset($_REQUEST['version']))
	die('0');

$sql = new mysqli('localhost', 'octgn', 'password', 'octgn');

if ($sql->connect_error) 
    die('0' . $sql->error);

$assembly = $_REQUEST['assembly'];
$name  = $_REQUEST['name'];
$error   = $_REQUEST['error'];
$version  = $_REQUEST['version'];

 
if($query = $sql->prepare("INSERT INTO `error_report`(`name`, `error`, `version`, `assembly`) VALUES(?,?,?,?);"))
{

	$query->bind_param("ssss",$name,$error,$version,$assembly); 

	$query->execute() or die('0\n' . $sql->error);
	die('1');
}
die('0' . $sql->error);

?>