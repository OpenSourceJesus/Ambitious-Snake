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
$publish = $connection->real_escape_string($_POST["publish"]);
$sql = "UPDATE `Maps` SET published='$publish' WHERE name='$mapName' AND username='$mapUsername' AND password='$mapPassword'";
if ($connection->query($sql) === TRUE)
{
	if ($publish == 0)
		die ("﬩Map unpublished﬩");
	die ("﬩Map published﬩");
}
die ("Map status didn't change: " . $connection->error)
?>

</body>
</html>