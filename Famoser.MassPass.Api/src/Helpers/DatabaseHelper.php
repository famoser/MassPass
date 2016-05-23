<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23.05.2016
 * Time: 15:25
 */

namespace Famoser\MassPass\Helpers;


use Slim\Container;

class DatabaseHelper
{
    /*
     * @var \PDO
     */
    private $database;

    private $container;

    public function __construct(Container $container)
    {
        $this->container = $container;
        $this->database = $this->container->get("db");
        echo "here" . (($this->database != null) ? "true" : "false");
        $this->initializeDatabase();
    }

    /**
     * @return \PDO
     */
    private function getConnection()
    {
        return $this->database;
    }

    private function initializeDatabase()
    {
        $tempFilePath = $this->container["settings"]["data_path"] . "/.db_created";
        if (file_exists($tempFilePath))
            return true;

        //create tables
        $conn = $this->getConnection();
        $conn->query("CREATE TABLE users (id int AUTO_INCREMENT)");

        file_put_contents($tempFilePath, time());

        return true;
    }

    private function createGuid()
    {
        return sprintf('%04X%04X-%04X-%04X-%04X-%04X%04X%04X', mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(16384, 20479), mt_rand(32768, 49151), mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(0, 65535));
    }
}