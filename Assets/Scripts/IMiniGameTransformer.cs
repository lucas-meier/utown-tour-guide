using Microsoft.Azure.ObjectAnchors.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMiniGameTransformer
{
    public void UpdateTransform(IObjectAnchorsServiceEventArgs e);
}
