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
$sql = "SELECT (name,username,par,data) FROM `Maps` WHERE published=1";
$result = $connection->query($sql);
for ($row = $result->fetch_assoc())
{
	echo ("Map name: " . $row["name"]);
	echo ("Map username: " . $row["username"]);
	echo ("Map par: " . $row["par"]);
	echo ("Map data: " . $row["data"] . "(END)");
}
die ("Didn't find map: " . $connection->error);
?>

</body>
</html>