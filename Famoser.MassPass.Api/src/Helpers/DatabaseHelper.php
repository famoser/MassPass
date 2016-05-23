<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23.05.2016
 * Time: 15:25
 */

namespace Famoser\MassPass\Helpers;


use Famoser\MassPass\Models\Entities\Base\EntityBase;
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
        $files = scandir(dirname(dirname(__FILE__)) . "/Assets/sql/initialize");
        foreach ($files as $file) {
            if (substr($file, -3) == "sql") {
                $queries = file_get_contents($file);
                $queryArray = explode(";", $queries);
                foreach ($queryArray as $item) {
                    if (trim($item) != "") {
                        $conn->query($item);
                    }
                }
            }
        }

        file_put_contents($tempFilePath, time());

        return true;
    }

    private function createGuid()
    {
        return sprintf('%04X%04X-%04X-%04X-%04X-%04X%04X%04X', mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(16384, 20479), mt_rand(32768, 49151), mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(0, 65535));
    }

    private function saveToDatabase(EntityBase $entity)
    {
        if ($entity->id > 0) {
            //update


        } else {
            //create
            $properties = (array)$entity;
            unset($properties["id"]);

            $sql = "INSERT INTO " . $entity->getTableName() . "(";
            foreach ($properties as $key => $val) {
                $sql .= $key . ",";
            }
            $sql = substr($sql, 0, -1);
            $sql .= ") VALUES (";
            foreach ($properties as $key => $val) {
                $sql .= ":" . $key . ",";
            }
            $sql = substr($sql, 0, -1);
            $sql .= ")";
            $request = $this->getConnection()->prepare($sql);
            if (!$request->execute($properties)) {
                return false;
            }
            $entity->id = $this->getConnection()->lastInsertId();
        }
        return true;
    }
}