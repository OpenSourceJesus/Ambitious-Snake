<!DOCTYPE HTML>
<html>
<body>

<?php
$serverName = $_POST["serverName"];
$serverUsername = $_POST["serverUsername"];
$serverPassword = $_POST["serverPassword"];
$databaseName = $_POST["databaseName"];
$connection = new mysqli($serverName, $serverUsername, $serverPassword, $databaseName);
if (mysqli_connect_error())
    die ("Error loading database: " . mysqli_connect_error());
else
	echo "Database loaded";
$mapName = $connection->real_escape_string($_POST["mapName"]);
$mapUsername = $connection->real_escape_string($_POST["mapUsername"]);
$mapPassword = $connection->real_escape_string($_POST["mapPassword"]);
$mapData = $connection->real_escape_string($_POST["mapData"]);
$parTime = $connection->real_escape_string($_POST["parTime"]);
if ($mapData == "")
	die ("No data to save");
$sql = "UPDATE `Maps` SET data='$mapData' AND par='$parTime' WHERE name='$mapName' AND username='$mapUsername' AND password='$mapPassword' AND published=0";
if ($connection->query($sql) === TRUE)
	die ("﬩Map updated﬩");
else
{
	echo "Didn't update map: " . $connection->error;
	$sql = "INSERT INTO `Maps` (name, data, username, password, published, par) VALUES ('$mapName', '$mapData', '$mapUsername', '$mapPassword', 0, '$parTime')";
	if ($connection->query($sql) === TRUE)
		die ("﬩Map saved﬩");
	die ("Didn't save: " . $connection->error);
}
?>

</body>
</html>