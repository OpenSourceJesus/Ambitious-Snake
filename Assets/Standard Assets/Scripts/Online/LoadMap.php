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
$sql = "SELECT data,par,published FROM `Maps` WHERE name='$mapName' AND username='$mapUsername' AND password='$mapPassword'";
$result = $connection->query($sql);
while ($row = $result->fetch_assoc())
	die ("﬩Map loaded﬩Map data: " . $row["data"] . "Map par: " . $row["par"] . "(END)");
die ("Didn't find map: " . $connection->error);
?>

</body>
</html>