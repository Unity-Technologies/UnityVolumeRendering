using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityVolumeRendering;

public class DtRawDatasetImporter : RawDatasetImporter
{
    static int k_DimX = 128;
    static int k_DimY = 256;
    static int k_DimZ = 256;
    static DataContentFormat k_ContentFormat = DataContentFormat.Uint8;
    static Endianness k_Endianness = Endianness.LittleEndian;
    static int k_SkipBytes = 0;

    byte[] m_Bytes;
    
    public DtRawDatasetImporter(byte[] bytes)
        : base(string.Empty, k_DimX, k_DimY, k_DimZ, k_ContentFormat, k_Endianness, k_SkipBytes)
    {
        m_Bytes = bytes;
    }

    public override VolumeDataset Import()
    {
        var stream = new MemoryStream(m_Bytes);
        var reader = new BinaryReader(stream);

        // Check that the dimension does not exceed the file size
        var expectedSize = (long)(k_DimX * k_DimY * k_DimZ) * GetSampleFormatSize(k_ContentFormat) + k_SkipBytes;
        if (stream.Length < expectedSize)
        {
            Debug.LogError($"The dimension({k_DimX}, {k_DimY}, {k_DimZ}) exceeds the file size. Expected file size is {expectedSize} bytes, while the actual file size is {stream.Length} bytes");
            reader.Close();
            stream.Close();
            return null;
        }

        var dataset = new VolumeDataset();
        dataset.datasetName = "RawImport"; // TODO
        dataset.filePath = String.Empty;
        dataset.dimX = k_DimX;
        dataset.dimY = k_DimY;
        dataset.dimZ = k_DimZ;

        // Skip header (if any)
        if (k_SkipBytes > 0)
            reader.ReadBytes(k_SkipBytes);

        var uDimension = k_DimX * k_DimY * k_DimZ;
        dataset.data = new float[uDimension];

        // Read the data/sample values
        for (int i = 0; i < uDimension; i++)
        {
            dataset.data[i] = (float)ReadDataValue(reader);
        }
        Debug.Log("Loaded dataset in range: " + dataset.GetMinDataValue() + "  -  " + dataset.GetMaxDataValue());

        reader.Close();
        stream.Close();

        dataset.FixDimensions();

        return dataset;
    }
    
}
