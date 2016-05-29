<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23.05.2016
 * Time: 15:25
 */

namespace Famoser\MassPass\Helpers;


use Famoser\MassPass\Models\Entities\AuthorizationCode;
use Famoser\MassPass\Models\Entities\Base\EntityBase;
use Famoser\MassPass\Models\Entities\Content;
use Famoser\MassPass\Models\Entities\Device;
use Famoser\MassPass\Models\Entities\User;
use Interop\Container\ContainerInterface;
use PDO;
use Slim\Container;

class DatabaseHelper
{
    /*
     * @var \PDO
     */
    private $database;

    private $container;

    public function __construct(ContainerInterface $container)
    {
        $this->container = $container;
        $this->initializeDatabase();
    }

    /**
     * @return \PDO
     */
    private function getConnection()
    {
        return $this->database;
    }

    private function constructPdo($path)
    {
        $pdo = new PDO("sqlite:" . $path);
        $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
        $pdo->setAttribute(PDO::ATTR_DEFAULT_FETCH_MODE, PDO::FETCH_ASSOC);
        return $pdo;
    }

    private function executeScripts(PDO $connection, $scriptsPath)
    {
        $files = scandir($scriptsPath);
        foreach ($files as $file) {
            if (substr($file, -3) == "sql") {
                $queries = file_get_contents($scriptsPath . "/" . $file);
                $queryArray = explode(";", $queries);
                foreach ($queryArray as $item) {
                    if (trim($item) != "") {
                        $connection->query($item);
                    }
                }
            }
        }
    }

    private static $activePathKey = 'path';

    public static function setPathKey($newPathKey)
    {
        DatabaseHelper::$activePathKey = $newPathKey;
    }

    public function initializeDatabase()
    {
        $activePath = $this->container["settings"]["data_path"] . "/" . $this->container['settings']['db'][DatabaseHelper::$activePathKey];

        $tempFilePath = $this->container["settings"]["data_path"] . "/.db_created";
        if (!file_exists($tempFilePath)) {

            $testPath = $this->container["settings"]["data_path"] . "/" . $this->container['settings']['db']['test_path'];
            $prodPath = $this->container["settings"]["data_path"] . "/" . $this->container['settings']['db']["path"];

            $scriptsPath = $this->container["settings"]["asset_path"] . "/sql/initialize";

            if (!file_exists($testPath))
                $this->executeScripts($this->constructPdo($testPath), $scriptsPath);
            if (!file_exists($prodPath))
                $this->executeScripts($this->constructPdo($prodPath), $scriptsPath);

            file_put_contents($tempFilePath, time());
        }

        $this->database = $this->constructPdo($activePath);
        return true;
    }

    private function createGuid()
    {
        return sprintf('%04X%04X-%04X-%04X-%04X-%04X%04X%04X', mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(16384, 20479), mt_rand(32768, 49151), mt_rand(0, 65535), mt_rand(0, 65535), mt_rand(0, 65535));
    }

    private function createQuery(EntityBase $entity, $where = null, $parameters = null, $orderBy = null, $limit = 1000)
    {
        $sql = "SELECT * FROM " . $entity->getTableName();
        if ($where != null) {
            $sql .= " WHERE " . $where;
        }
        if ($orderBy != null) {
            $sql .= " ORDER BY " . $orderBy;
        }
        $sql .= " LIMIT " . $limit;
        return $sql;
    }

    private function executeAndFetch(EntityBase $entity, $sql, $parameters)
    {
        try {
            file_put_contents("debug.txt", $sql . " params: " . json_encode($parameters));
            $request = $this->getConnection()->prepare($sql);
            if (!$request->execute($parameters)) {
                return false;
            }
            return $request->fetchAll(PDO::FETCH_CLASS, get_class($entity));
        } catch (\Exception $ex) {
            LogHelper::log($ex->getMessage() . "     " . $ex->getTraceAsString() . "     " . $sql . "     " . json_encode($parameters), "DatabaseHelper.txt");
        }
        return null;
    }

    /**
     * @param EntityBase $entity
     * @param null $where
     * @param null $parameters
     * @param null $orderBy
     * @param int $limit
     * @return AuthorizationCode[]|Content[]|Device[]|User[]|bool
     */
    public function getFromDatabase(EntityBase $entity, $where = null, $parameters = null, $orderBy = null, $limit = 1000)
    {
        $sql = $this->createQuery($entity, $where, $parameters, $orderBy, $limit);
        $res = $this->executeAndFetch($entity, $sql, $parameters);
        return $res;
    }

    /**
     * @param EntityBase $entity
     * @param null $where
     * @param null $parameters
     * @param null $orderBy
     * @param int $limit
     * @return AuthorizationCode|Content|Device|User|bool
     */
    public function getSingleFromDatabase(EntityBase $entity, $where = null, $parameters = null, $orderBy = null, $limit = 1000)
    {
        $sql = $this->createQuery($entity, $where, $parameters, $orderBy, $limit);
        $res = $this->executeAndFetch($entity, $sql, $parameters);
        if (count($res) > 0)
            return $res[0];
        return null;
    }

    /**
     * @param EntityBase $entity
     * @return bool
     */
    public function saveToDatabase(EntityBase $entity)
    {
        $properties = (array)$entity;
        LogHelper::log(json_encode($properties, JSON_PRETTY_PRINT) . "\n\n\n" . json_encode($entity, JSON_PRETTY_PRINT), "DatabaseHelper_" . $entity->getTableName() . '_' . time() . "_" . uniqid() . ".txt");
        unset($properties["id"]);
        if ($entity->id > 0) {
            //update
            $sql = "UPDATE " . $entity->getTableName() . " SET ";
            foreach ($properties as $key => $val) {
                $sql .= $key . "=:" . $key . ",";
            }
            $sql = substr($sql, 0, -1);
            $sql .= " WHERE id=:id";
            $request = $this->getConnection()->prepare($sql);
            if (!$request->execute($properties)) {
                return false;
            }

        } else {
            //create
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

    /**
     * @param EntityBase $entity
     * @return bool
     */
    public function deleteFromDatabase(EntityBase $entity)
    {
        $sql = "DELETE FROM " . $entity->getTableName() . " WHERE id=:id";
        $params = array("id" => $entity->id);
        $prep = $this->getConnection()->prepare($sql);
        return $prep->execute($params);
    }
}