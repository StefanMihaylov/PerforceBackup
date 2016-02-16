﻿namespace PerforceBackup.Engine.Interfaces
{
    using PerforceBackup.Engine.Models;

    public interface IRootArhivator : IService
    {
        void AddFileToArhiv(string sourceFullPath, string arhiveFullPath);

        ArhiveModel Compress(CheckpointModel checkpoint, IArhiveSettings settings);
    }
}
