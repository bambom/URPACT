using UnityEngine;
using System.Collections;

public class DownTile : MonoBehaviour
{
    Material mDownTileMaterial;
    float mCurrentPercent = 1.0f;
    float mTargetPercent = 1.0f;
    const float MatchSpeed = 1.0f;

    // Use this for initialization
    void Start()
    {
        MeshRenderer render = GetComponent<MeshRenderer>();
        mDownTileMaterial = render ? render.material : null;
    }

    // Update is called once per frame
    void Update()
    {
        if (mCurrentPercent == mTargetPercent)
            return;

        float alpha = MatchSpeed * Time.deltaTime / Mathf.Abs(mTargetPercent - mCurrentPercent);
        if (alpha >= 1.0f)
            mCurrentPercent = mTargetPercent;
        else
            mCurrentPercent = Mathf.Lerp(mCurrentPercent, mTargetPercent, alpha);

        if (mDownTileMaterial)
            mDownTileMaterial.SetTextureOffset("_MainTex", new Vector2(1.0f - mCurrentPercent, 0));
    }

    public void OnSoulChanged(int curValue, int maxValue)
    {
        mTargetPercent = (float)curValue / maxValue;
    }
}
