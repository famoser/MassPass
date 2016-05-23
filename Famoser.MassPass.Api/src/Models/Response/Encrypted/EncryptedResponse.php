<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 23.05.2016
 * Time: 09:48
 */

namespace Famoser\MassPass\Models\Response\Encrypted;


use Famoser\MassPass\Models\Response\Base\ApiResponse;

class EncryptedResponse extends ApiResponse
{
    public $DownloadUri;
}