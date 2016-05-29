<?php
/**
 * Created by PhpStorm.
 * User: famoser
 * Date: 29/05/2016
 * Time: 19:28
 */

namespace Famoser\MassPass\Types;


class RemoteStatus
{
//[Description("up to date")]
    const UpToDate = 1;
//[Description("changed")]
    const Changed = 2;
//[Description("not found on server")]
    const NotFound = 3;
}