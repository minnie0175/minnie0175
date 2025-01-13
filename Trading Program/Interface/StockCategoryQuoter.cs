using CommonLib.DBDataType;
using System.Collections.Generic;
using WeifenLuo.WinFormsUI.Docking;

namespace Ciri.Interface
{
    public interface StockCategoryQuoter
    {
        void SetCategory(string selectCategory, string subCategoryName);

        void SetSortingData(Dictionary<int, SortingDataDoc> sortingDataDic);

        void SetSelectPurpose(string purpose, string style);

        void SetBasketHedgeQuoterMode();

        void Start(DockPanel panel);
    }
}
