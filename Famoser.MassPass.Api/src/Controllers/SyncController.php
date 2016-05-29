<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 29/05/2016
 * Time: 18:30
 */

namespace Famoser\MassPass\Controllers;


use Famoser\MassPass\Helpers\FormatHelper;
use Famoser\MassPass\Helpers\RequestHelper;
use Famoser\MassPass\Helpers\ResponseHelper;
use Famoser\MassPass\Models\Entities\Content;
use Famoser\MassPass\Models\Entities\ContentHistory;
use Famoser\MassPass\Models\Entities\Device;
use Famoser\MassPass\Models\Entities\User;
use Famoser\MassPass\Models\Request\CollectionEntriesRequest;
use Famoser\MassPass\Models\Request\Entities\RefreshEntity;
use Famoser\MassPass\Models\Response\Base\ApiResponse;
use Famoser\MassPass\Models\Response\CollectionEntriesResponse;
use Famoser\MassPass\Models\Response\ContentEntityHistoryResponse;
use Famoser\MassPass\Models\Response\DownloadContentEntityResponse;
use Famoser\MassPass\Models\Response\Entities\CollectionEntryEntity;
use Famoser\MassPass\Models\Response\Entities\HistoryEntry;
use Famoser\MassPass\Models\Response\RefreshResponse;
use Famoser\MassPass\Models\Response\UpdateResponse;
use Famoser\MassPass\Types\ApiErrorTypes;
use Famoser\MassPass\Types\RemoteStatus;
use Slim\Http\Request;
use Slim\Http\Response;
use Upload\File;
use Upload\Storage\FileSystem;

class SyncController extends BaseController
{
    public function refresh(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseRefreshRequest($request);
        if ($this->isAuthorized($model)) {
            if (!$this->isWellDefined($model, null, array("RefreshEntities")))
                return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotWellDefined));
            
            $guids = [];
            for ($i = 0; $i < count($model->RefreshEntities); $i++) {
                $guids[] = $model->RefreshEntities[$i]->ServerId;
            }
            $helper = $this->getDatabaseHelper();
            $results = $helper->getWithInFromDatabase(new Content(), "guid", $guids);

            $resp = new RefreshResponse();
            $resp->RefreshEntities = [];
            for ($i = 0; $i < count($results); $i++) {
                $found = false;
                for ($j = 0; $j < count($model->RefreshEntities); $j++) {
                    if ($model->RefreshEntities[$j] == $results[$i]->guid) {
                        $found = true;
                        if ($model->RefreshEntities[$j]->VersionId != $results[$i]->version_id) {
                            $entity = new RefreshEntity();
                            $entity->VersionId = $results[$i]->version_id;
                            $entity->ServerId = $results[$i]->guid;
                            $entity->RemoteStatus = RemoteStatus::Changed;
                            $resp->RefreshEntities[] = $entity;
                        }
                        break;
                    }
                }
                if (!$found) {
                    $entity = new RefreshEntity();
                    $entity->VersionId = $results[$i]->version_id;
                    $entity->ServerId = $results[$i]->guid;
                    $entity->RemoteStatus = RemoteStatus::NotFound;
                    $resp->RefreshEntities[] = $entity;
                }
            }

            return ResponseHelper::getJsonResponse($response, $resp);
        } else {
            return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotAuthorized));
        }
    }

    public function update(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseUpdateRequest($request);
        if ($this->isAuthorized($model)) {
            if (!$this->isWellDefined($model, array("RelationId", "ServerId")))
                return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotWellDefined));

            $resp = new UpdateResponse();
            $helper = $this->getDatabaseHelper();
            $newVersion = $helper->createUniqueVersion();

            //save file
            $storage = new FileSystem($this->getUserDirForContent($this->getAuthorizedUser($model)->guid));
            $file = new File('updateFile', $storage);
            $file->setName($this->getFilenameForContent($model->ServerId, $newVersion));
            $file->upload();

            $contentId = null;
            //update database
            $exiting = $helper->getSingleFromDatabase(new Content(), "guid=:guid", array("guid" => $model->ServerId));
            if ($exiting != null) {
                $exiting->version_id = $newVersion;

                if (!$helper->saveToDatabase($exiting)) {
                    return $this->returnFailure(BaseController::DATABASE_FAILURE, $response);
                }
                $contentId = $exiting->id;
            } else {
                $newModel = new Content();
                $newModel->guid = $model->ServerId;
                $newModel->user_id = $this->getAuthorizedUser($model)->guid;
                $newModel->relation_guid = $model->RelationId;
                $newModel->version_id = $newVersion;

                if (!$helper->saveToDatabase($newModel)) {
                    return $this->returnFailure(BaseController::DATABASE_FAILURE, $response);
                }
                $contentId = $newModel->id;
            }

            //save
            $newModel = new ContentHistory();
            $newModel->version_id = $newVersion;
            $newModel->content_id = $contentId;
            $newModel->creation_date_time = time();
            $newModel->device_id = $this->getAuthorizedDevice($model)->id;
            if (!$helper->saveToDatabase($newModel)) {
                return $this->returnFailure(BaseController::DATABASE_FAILURE, $response);
            }

            $resp->ServerRelationId = $model->RelationId;
            $resp->ServerId = $model->ServerId;
            $resp->VersionId = $newVersion;
            return ResponseHelper::getJsonResponse($response, $resp);
        } else {
            return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotAuthorized));
        }
    }
    
    public function readContentEntity(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseContentEntityRequest($request);
        if ($this->isAuthorized($model)) {
            if (!$this->isWellDefined($model, array("VersionId", "ServerId")))
                return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotWellDefined));

            $path = $this->getPathForContent($this->getAuthorizedUser($model)->guid, $model->ServerId, $model->VersionId);
            if (!file_exists($path)) {
                $resp = new ApiResponse(false, ApiErrorTypes::ContentNotFound);
                return ResponseHelper::getJsonResponse($response, $resp);
            }
            $resp = new DownloadContentEntityResponse();
            $content = file_get_contents($path);
            return $response->getBody()->write($content);
        } else {
            return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotAuthorized));
        }
    }

    public function readCollectionEntries(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseCollectionEntriesRequest($request);
        if ($this->isAuthorized($model)) {
            if (!$this->isWellDefined($model, array("Guid"), array("KnownServerIds")))
                return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotWellDefined));

            $guids = [];
            for ($i = 0; $i < count($model->KnownServerIds); $i++) {
                $guids[] = $model->KnownServerIds[$i];
            }

            $helper = $this->getDatabaseHelper();
            $contents = $helper->getWithInFromDatabase(new Content(), "guid", $guids, true, "relation_id=:relation_id", array("relation_id" => $model->Guid));

            $resp = new CollectionEntriesResponse();
            $resp->CollectionEntryEntities = [];
            foreach ($contents as $content) {
                $entity = new CollectionEntryEntity();
                $entity->VersionId = $content->version_id;
                $entity->ServerId = $content->guid;
                $resp->CollectionEntryEntities[] = $entity;
            }

            return ResponseHelper::getJsonResponse($response, $resp);
        } else {
            return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotAuthorized));
        }
    }

    public function getHistory(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseContentEntityHistoryRequest($request);
        if ($this->isAuthorized($model)) {
            if (!$this->isWellDefined($model, array("ServerId")))
                return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotWellDefined));

            $helper = $this->getDatabaseHelper();
            $collection = $helper->getSingleFromDatabase(new Content(), "guid=:guid", array("guid" => $model->ServerId));
            if ($collection != null) {
                //get all entries
                $historyEntries = $helper->getFromDatabase(new ContentHistory(), "content_id=:content_id", array("content_id" => $collection->id), "creation_date_time DESC");

                //cache all devices
                $ids = [];
                for ($i = 0; $i < count($historyEntries); $i++) {
                    $ids[] = $historyEntries[$i]->user_id;
                }
                $helper = $this->getDatabaseHelper();
                $devices = $helper->getWithInFromDatabase(new Device(), "id", $ids);

                //create response
                $resp = new ContentEntityHistoryResponse();
                $resp->HistoryEntries = [];
                foreach ($historyEntries as $historyEntry) {
                    $entity = new HistoryEntry();
                    $entity->CreationDateTime = FormatHelper::toCSharpDateTime($historyEntry->creation_date_time);
                    $entity->VersionId = $historyEntry->version_id;
                    foreach ($devices as $device) {
                        if ($device->id == $historyEntry->device_id) {
                            $entity->DeviceId = $device->guid;
                            break;
                        }
                    }
                    $resp->HistoryEntries[] = $entity;
                }
                return ResponseHelper::getJsonResponse($response, $resp);
            } else {
                $resp = new ApiResponse(false, ApiErrorTypes::ContentNotFound);
                return ResponseHelper::getJsonResponse($response, $resp);
            }
        } else {
            return ResponseHelper::getJsonResponse($response, new ApiResponse(false, ApiErrorTypes::NotAuthorized));
        }
    }

    private function getPathForContent($userGuid, $contentGuid, $version)
    {
        return $this->getUserDirForContent($userGuid) . "/" . $this->getFilenameForContent($contentGuid, $version);
    }

    private function getFilenameForContent($contentGuid, $version)
    {
        return $contentGuid . "_" . $version;
    }

    private function getUserDirForContent($userGuid)
    {
        $path = $this->container->get("settings")["file_path"] . "/" . $userGuid;
        if (!is_dir($path)) {
            mkdir($path);
        }
        return $path;
    }
}