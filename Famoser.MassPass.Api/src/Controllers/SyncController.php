<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 29/05/2016
 * Time: 18:30
 */

namespace Famoser\MassPass\Controllers;


use Famoser\MassPass\Helpers\FormatHelper;
use Famoser\MassPass\Helpers\GuidHelper;
use Famoser\MassPass\Helpers\RequestHelper;
use Famoser\MassPass\Helpers\ResponseHelper;
use Famoser\MassPass\Models\Entities\Content;
use Famoser\MassPass\Models\Entities\ContentHistory;
use Famoser\MassPass\Models\Entities\Device;
use Famoser\MassPass\Models\Entities\User;
use Famoser\MassPass\Models\Request\CollectionEntriesRequest;
use Famoser\MassPass\Models\Response\Base\ApiResponse;
use Famoser\MassPass\Models\Response\CollectionEntriesResponse;
use Famoser\MassPass\Models\Response\ContentEntityHistoryResponse;
use Famoser\MassPass\Models\Response\Entities\CollectionEntryEntity;
use Famoser\MassPass\Models\Response\Entities\HistoryEntry;
use Famoser\MassPass\Models\Response\Entities\RefreshEntity;
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
                return $this->returnApiError(ApiErrorTypes::NotWellDefined, $response);

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
                    if ($model->RefreshEntities[$j]->ServerId == $results[$i]->guid) {
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
            return $this->returnApiError(ApiErrorTypes::NotAuthorized, $response);
        }
    }

    public function update(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseUpdateRequest($request);
        if ($this->isAuthorized($model)) {
            if (!$this->isWellDefined($model, array("RelationId")))
                return $this->returnApiError(ApiErrorTypes::NotWellDefined, $response);

            $resp = new UpdateResponse();
            $helper = $this->getDatabaseHelper();
            $versionId = GuidHelper::createGuid();
            $serverId = $model->ServerId;

            //save file
            $storage = new FileSystem($this->getUserDirForContent($this->getAuthorizedUser($model)->guid));
            $file = new File('updateFile', $storage);
            $file->setName($this->getFilenameForContent($model->ServerId, $versionId));
            $file->upload();

            //update database
            $contentId = null;
            $exiting = null;
            if (GuidHelper::isGuidValid($serverId))
                $exiting = $helper->getSingleFromDatabase(new Content(), "guid=:guid", array("guid" => $serverId));
            if ($exiting != null) {
                if ($exiting->version_id != $model->VersionId) {
                    return $this->returnApiError(ApiErrorTypes::InvalidVersionId, $response);
                }

                $exiting->version_id = $versionId;

                if (!$helper->saveToDatabase($exiting)) {
                    return $this->returnApiError(ApiErrorTypes::DatabaseFailure, $response);
                }
                $contentId = $exiting->id;
            } else {
                $serverId = GuidHelper::createGuid();
                $newModel = new Content();
                $newModel->guid = $serverId;
                $newModel->user_id = $this->getAuthorizedUser($model)->guid;
                $newModel->relation_id = $model->RelationId;
                $newModel->version_id = $versionId;

                if (!$helper->saveToDatabase($newModel)) {
                    return $this->returnApiError(ApiErrorTypes::DatabaseFailure, $response);
                }
                $contentId = $newModel->id;
            }

            //save to history
            $newModel = new ContentHistory();
            $newModel->version_id = $versionId;
            $newModel->content_id = $contentId;
            $newModel->creation_date_time = time();
            $newModel->device_id = $this->getAuthorizedDevice($model)->id;
            if (!$helper->saveToDatabase($newModel)) {
                return $this->returnApiError(ApiErrorTypes::DatabaseFailure, $response);
            }

            $resp->ServerRelationId = $model->RelationId;
            $resp->ServerId = $serverId;
            $resp->VersionId = $versionId;
            return ResponseHelper::getJsonResponse($response, $resp);
        } else {
            return $this->returnApiError(ApiErrorTypes::NotAuthorized, $response);
        }
    }

    public function readContentEntity(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseContentEntityRequest($request);
        if ($this->isAuthorized($model)) {
            if (!$this->isWellDefined($model, array("VersionId", "ServerId")))
                return $this->returnApiError(ApiErrorTypes::NotWellDefined, $response);

            $path = $this->getPathForContent($this->getAuthorizedUser($model)->guid, $model->ServerId, $model->VersionId);
            if (!file_exists($path)) {
                return $this->returnApiError(ApiErrorTypes::ContentNotFound, $response, $path);
            }
            $content = file_get_contents($path);
            return $response->getBody()->write($content);
        } else {
            return $this->returnApiError(ApiErrorTypes::NotAuthorized, $response);
        }
    }

    public function readCollectionEntries(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseCollectionEntriesRequest($request);
        if ($this->isAuthorized($model)) {
            if (!$this->isWellDefined($model, array("RelationId"), array("KnownServerIds")))
                return $this->returnApiError(ApiErrorTypes::NotWellDefined, $response);

            $helper = $this->getDatabaseHelper();
            if (count($model->KnownServerIds) > 0) {
                $guids = [];
                for ($i = 0; $i < count($model->KnownServerIds); $i++) {
                    $guids[] = $model->KnownServerIds[$i];
                }

                $contents = $helper->getWithInFromDatabase(new Content(), "guid", $guids, true, "relation_id=:relation_id", array("relation_id" => $model->RelationId));
            } else {
                $contents = $helper->getFromDatabase(new Content(), "relation_id=:relation_id", array("relation_id" => $model->RelationId));
            }

            $resp = new CollectionEntriesResponse();
            $resp->CollectionEntryEntities = [];
            if ($contents != null) {
                foreach ($contents as $content) {
                    $entity = new CollectionEntryEntity();
                    $entity->VersionId = $content->version_id;
                    $entity->ServerId = $content->guid;
                    $entity->RelationId = $content->relation_id;
                    $resp->CollectionEntryEntities[] = $entity;
                }
            }

            return ResponseHelper::getJsonResponse($response, $resp);
        } else {
            return $this->returnApiError(ApiErrorTypes::NotAuthorized, $response);
        }
    }

    public function getHistory(Request $request, Response $response, $args)
    {
        $model = RequestHelper::parseContentEntityHistoryRequest($request);
        if ($this->isAuthorized($model)) {
            if (!$this->isWellDefined($model, array("ServerId")))
                return $this->returnApiError(ApiErrorTypes::NotWellDefined, $response);

            $helper = $this->getDatabaseHelper();
            $collection = $helper->getSingleFromDatabase(new Content(), "guid=:guid", array("guid" => $model->ServerId));
            if ($collection != null) {
                //get all entries
                $historyEntries = $helper->getFromDatabase(new ContentHistory(), "content_id=:content_id", array("content_id" => $collection->id), "creation_date_time DESC");

                //cache all devices
                $ids = [];
                for ($i = 0; $i < count($historyEntries); $i++) {
                    $ids[] = $historyEntries[$i]->device_id;
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
                return $this->returnApiError(ApiErrorTypes::ContentNotFound, $response);
            }
        } else {
            return $this->returnApiError(ApiErrorTypes::NotAuthorized, $response);
        }
    }

    private function getPathForContent($userGuid, $contentGuid, $version)
    {
        return $this->getUserDirForContent($userGuid) . "/" . $this->getFilenameForContent($contentGuid, $version) . "." . $this->getExtensionForContent();
    }

    private function getFilenameForContent($contentGuid, $version)
    {
        return $contentGuid . "_" . $version;
    }

    private function getExtensionForContent()
    {
        //empty fileextension
        return "";
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