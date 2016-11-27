/**
 * Copyright 2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace biz.dfch.CS.Commons
{
    public partial class Constants
    {
        public static partial class Validation
        {
            public static partial class Currency
            {
                // to get a list of all current currency codes use these PoSH statements:
                /*
$itemsXml = Invoke-RestMethod http://www.currency-iso.org/dam/downloads/lists/list_one.xml
$items = $itemsXml.ISO_4217.CcyTbl.CcyNtry.Ccy | Sort | Get-Unique

$output = New-Object System.Text.StringBuilder;
$c = 0;
while($c -lt $items.Count)
{ 
	$line = New-Object System.Text.StringBuilder;
	do
	{
		$item = $items[$c]; 
		if(!$item) 
		{ 
			$c++;
			continue;
		}
	
		$null = $line.Append('"');
		$null = $line.Append($item);
		$null = $line.Append('"');
		$null = $line.Append(',');
		$c++;
	}
	while(80 -gt $line.Length -and $c -lt $items.Count);
	$null = $output.AppendLine($line.ToString());
}
$output.ToString();
                 */ 
                public static readonly string[] CurrencyCodes =
                {
"AED","AFN","ALL","AMD","ANG","AOA","ARS","AUD","AWG","AZN","BAM","BBD","BDT","BGN",
"BHD","BIF","BMD","BND","BOB","BOV","BRL","BSD","BTN","BWP","BYN","BYR","BZD","CAD",
"CDF","CHE","CHF","CHW","CLF","CLP","CNY","COP","COU","CRC","CUC","CUP","CVE","CZK",
"DJF","DKK","DOP","DZD","EGP","ERN","ETB","EUR","FJD","FKP","GBP","GEL","GHS","GIP",
"GMD","GNF","GTQ","GYD","HKD","HNL","HRK","HTG","HUF","IDR","ILS","INR","IQD","IRR",
"ISK","JMD","JOD","JPY","KES","KGS","KHR","KMF","KPW","KRW","KWD","KYD","KZT","LAK",
"LBP","LKR","LRD","LSL","LYD","MAD","MDL","MGA","MKD","MMK","MNT","MOP","MRO","MUR",
"MVR","MWK","MXN","MXV","MYR","MZN","NAD","NGN","NIO","NOK","NPR","NZD","OMR","PAB",
"PEN","PGK","PHP","PKR","PLN","PYG","QAR","RON","RSD","RUB","RWF","SAR","SBD","SCR",
"SDG","SEK","SGD","SHP","SLL","SOS","SRD","SSP","STD","SVC","SYP","SZL","THB","TJS",
"TMT","TND","TOP","TRY","TTD","TWD","TZS","UAH","UGX","USD","USN","UYI","UYU","UZS",
"VEF","VND","VUV","WST","XAF","XAG","XAU","XBA","XBB","XBC","XBD","XCD","XDR","XOF",
"XPD","XPF","XPT","XSU","XTS","XUA","XXX","YER","ZAR","ZMW","ZWL",
                };
            }
        }
    }
}
