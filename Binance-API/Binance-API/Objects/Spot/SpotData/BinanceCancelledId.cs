/*
*MIT License
*
*Copyright (c) 2022 S Christison
*
*Permission is hereby granted, free of charge, to any person obtaining a copy
*of this software and associated documentation files (the "Software"), to deal
*in the Software without restriction, including without limitation the rights
*to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
*copies of the Software, and to permit persons to whom the Software is
*furnished to do so, subject to the following conditions:
*
*The above copyright notice and this permission notice shall be included in all
*copies or substantial portions of the Software.
*
*THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
*IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
*FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
*AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
*LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
*OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
*SOFTWARE.
*/

using Newtonsoft.Json;

namespace BinanceAPI.Objects.Spot.SpotData
{
    /// <summary>
    /// Ids of a canceled order, either OCO or normal
    /// </summary>
    public class BinanceCancelledId
    {
        /// <summary>
        /// Id of the order
        /// </summary>
        [JsonProperty("orderId")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Client order id
        /// </summary>
        public string ClientOrderId { get; set; } = string.Empty;

        [JsonProperty("listClientOrderId")]
        private string ListClientOrderId
        {
            get => Id;
            set
            {
                ClientOrderId = value;
                OcoOrder = true;
            }
        }

        [JsonProperty("orderListId")]
        private string OrderListId
        {
            get => Id;
            set
            {
                if (value == "-1")
                    return;

                Id = value;
                OcoOrder = true;
            }
        }

        /// <summary>
        /// Whether or not it is an OCO order
        /// </summary>
        public bool OcoOrder { get; set; }
    }
}
