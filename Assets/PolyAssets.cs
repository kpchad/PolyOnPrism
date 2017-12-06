﻿// Copyright 2017 Google Inc. All rights reserved.
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

    // Text field where we display the attributions (credits) for the assets we display.
    public Text attributionsText;

    // Status bar text.
    public Text statusText;

    // thumbnails to render
    public Material firstThumbnail;
    public Material secondThumbnail;
    public Material thirdThumbnail;
    public Material fourthThumbnail;

    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;

    List<PolyAsset> assetsInPalette = new List<PolyAsset>();

    // Number of assets imported so far.
    private int assetCount = 0;

    private void Start() {
        // display featured asset thumbnais
        Debug.Log("Getting featured asset thumbnails...");
        statusText.text = "Requesting...";

        PolyListAssetsRequest request = PolyListAssetsRequest.Featured();
        // Limit requested models to those of medium complexity or lower.
        request.maxComplexity = PolyMaxComplexityFilter.MEDIUM;
        PolyApi.ListAssets(request, ListAssetsCallback);

        // setup add asset buttons
        button1.onClick.AddListener(delegate { ImportAsset(0); });
        button2.onClick.AddListener(delegate { ImportAsset(1); });
        button3.onClick.AddListener(delegate { ImportAsset(2); });
        button4.onClick.AddListener(delegate { ImportAsset(3); });
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

        // Now let's get the first 4 featured assets and show their thumbnails
        for (int i = 0; i < Mathf.Min(4, result.Value.assets.Count); i++) {
            // fetch this asset's thumbnail
            PolyApi.FetchThumbnail(result.Value.assets[i], FetchThumbnailCallback);
            assetsInPalette.Add(result.Value.assets[i]);
        }

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
        statusText.text = "pick an asset to import";
    }

    void ImportAsset(int idx) {

        List<PolyAsset> assetsInUse = new List<PolyAsset>();

        // Set the import options.
        PolyImportOptions options = PolyImportOptions.Default();
        // We want to rescale the imported meshes to a specific size.
        options.rescalingMode = PolyImportOptions.RescalingMode.FIT;
        // The specific size we want assets rescaled to (fit in a 1x1x1 box):
        options.desiredSize = 1.0f;
        // We want the imported assets to be recentered such that their centroid coincides with the origin:
        options.recenter = true;

        Debug.Log("import asset " + idx);
        PolyApi.Import(assetsInPalette[idx],options,ImportAssetCallback);
        assetsInUse.Add(assetsInPalette[idx]);

        // Show attributions for the assets we display.
        //attributionsText.text = PolyApi.GenerateAttributions(includeStatic: true, runtimeAssets: assetsInUse);
    }

    // Callback invoked when an asset has just been imported.
    private void ImportAssetCallback(PolyAsset asset, PolyStatusOr<PolyImportResult> result) {
        if (!result.Ok) {
            Debug.LogError("Failed to import asset. :( Reason: " + result.Status);
            return;
        }

        // Position each asset evenly spaced from the next.
        assetCount++;
        result.Value.gameObject.transform.position = new Vector3((assetCount * 1.5f - 1.5f), 0f, 3f);

        //statusText.text = "Imported " + assetCount + " assets";
    }
}        