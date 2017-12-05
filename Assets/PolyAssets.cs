// Copyright 2017 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using PolyToolkit;

/// <summary>
/// Example that shows the top 5 featured models.
/// 
/// This example requests the list of featured assets from Poly. Then it imports the top 5
/// into the scene, side by side.
/// </summary>
public class PolyAssets : MonoBehaviour {

    // ATTENTION: Before running this example, you must set your API key in Poly Toolkit settings.
    //   1. Click "Poly | Poly Toolkit Settings..."
    //      (or select PolyToolkit/Resources/PtSettings.asset in the editor).
    //   2. Click the "Runtime" tab.
    //   3. Enter your API key in the "Api key" box.
    //
    // This example does not use authentication, so there is no need to fill in a Client ID or Client Secret.

    // Number of assets imported so far.
    private int assetCount = 0;

    // Text field where we display the attributions (credits) for the assets we display.
    public Text attributionsText;

    // Status bar text.
    public Text statusText;

    // thumbnails to render
    public Material firstThumbnail;
    public Material secondThumbnail;
    public Material thirdThumbnail;
    public Material fourthThumbnail;

    List<PolyAsset> assetsInPalette = new List<PolyAsset>();

    private void Start() {
        // Request a list of featured assets from Poly.
        Debug.Log("Getting featured assets...");
        statusText.text = "Requesting...";

        PolyListAssetsRequest request = PolyListAssetsRequest.Featured();
        // Limit requested models to those of medium complexity or lower.
        request.maxComplexity = PolyMaxComplexityFilter.MEDIUM;
        PolyApi.ListAssets(request, ListAssetsCallback);
    }

    // Callback invoked when the featured assets results are returned.
    private void ListAssetsCallback(PolyStatusOr<PolyListAssetsResult> result) {
        if (!result.Ok) {
            Debug.LogError("Failed to get featured assets. :( Reason: " + result.Status);
            statusText.text = "ERROR: " + result.Status;
            return;
        }

        Debug.Log("Successfully got featured assets!");
        statusText.text = "Importing thumbnails...";

        // Now let's get the first 5 featured assets and show their thumbnails
        for (int i = 0; i < Mathf.Min(4, result.Value.assets.Count); i++) {
            // fetch this asset's thumbnail
            PolyApi.FetchThumbnail(result.Value.assets[i], FetchThumbnailCallback);
            assetsInPalette.Add(result.Value.assets[i]);
        }

        //    // Show attributions for the assets we display.
        //    attributionsText.text = PolyApi.GenerateAttributions(includeStatic: true, runtimeAssets: assetsInUse);
    }

    int thumbnailCount = 0;
    // Callback invoked when a thumbnail has just been fetched.
    private void FetchThumbnailCallback(PolyAsset asset, PolyStatus result) {
        if (!result.ok) {
            Debug.LogError("Failed to import thumbnail. :( Reason: " + result.errorMessage);
            return;
        }
        if (thumbnailCount == 0){
            firstThumbnail.mainTexture = assetsInPalette[0].thumbnailTexture;
        }
        else if (thumbnailCount == 1){
            secondThumbnail.mainTexture = assetsInPalette[1].thumbnailTexture;
        }
        else if(thumbnailCount == 2) {
            thirdThumbnail.mainTexture = assetsInPalette[2].thumbnailTexture;
        }
        else if (thumbnailCount == 3){
            fourthThumbnail.mainTexture = assetsInPalette[3].thumbnailTexture;
        }
        else{
        }
        thumbnailCount++;
    }
}