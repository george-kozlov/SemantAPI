//
// SemantAPI.Robot, SemantAPI.Human
// Copyright (C) 2013 George Kozlov
// These programs are free software: you can redistribute them and/or modify them under the terms of the GNU General Public License as published by the Free Software Foundation. either version 3 of the License, or any later version.
// These programs are distributed in the hope that they will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details. You should have received a copy of the GNU General Public License along with this program. If not, see http://www.gnu.org/licenses/.
// For further questions or inquiries, please contact semantapi (at) gmail (dot) com
//

using System;
using System.Collections.Generic;

namespace SemantAPI.Common
{
	public sealed class LocaleHelper
	{
		#region Private members

		static Dictionary<string, string> _abbreviations = null;
		static Dictionary<string, string> _countries = null;

		#endregion

		#region Constructor

		static LocaleHelper()
		{
			_abbreviations = new Dictionary<string, string>(250);
			_abbreviations.Add("AF", "AFGHANISTAN");
			_abbreviations.Add("AX", "ALAND ISLANDS");
			_abbreviations.Add("AL", "ALBANIA");
			_abbreviations.Add("DZ", "ALGERIA");
			_abbreviations.Add("AS", "AMERICAN SAMOA");
			_abbreviations.Add("AD", "ANDORRA");
			_abbreviations.Add("AO", "ANGOLA");
			_abbreviations.Add("AI", "ANGUILLA");
			_abbreviations.Add("AQ", "ANTARCTICA");
			_abbreviations.Add("AG", "ANTIGUA AND BARBUDA");
			_abbreviations.Add("AR", "ARGENTINA");
			_abbreviations.Add("AM", "ARMENIA");
			_abbreviations.Add("AW", "ARUBA");
			_abbreviations.Add("AU", "AUSTRALIA");
			_abbreviations.Add("AT", "AUSTRIA");
			_abbreviations.Add("AZ", "AZERBAIJAN");
			_abbreviations.Add("BS", "BAHAMAS");
			_abbreviations.Add("BH", "BAHRAIN");
			_abbreviations.Add("BD", "BANGLADESH");
			_abbreviations.Add("BB", "BARBADOS");
			_abbreviations.Add("BY", "BELARUS");
			_abbreviations.Add("BE", "BELGIUM");
			_abbreviations.Add("BZ", "BELIZE");
			_abbreviations.Add("BJ", "BENIN");
			_abbreviations.Add("BM", "BERMUDA");
			_abbreviations.Add("BT", "BHUTAN");
			_abbreviations.Add("BO", "BOLIVIA PLURINATIONAL STATE OF");
			_abbreviations.Add("BQ", "BONAIRE SINT EUSTATIUS AND SABA");
			_abbreviations.Add("BA", "BOSNIA AND HERZEGOVINA");
			_abbreviations.Add("BW", "BOTSWANA");
			_abbreviations.Add("BV", "BOUVET ISLAND");
			_abbreviations.Add("BR", "BRAZIL");
			_abbreviations.Add("IO", "BRITISH INDIAN OCEAN TERRITORY");
			_abbreviations.Add("BN", "BRUNEI DARUSSALAM");
			_abbreviations.Add("BG", "BULGARIA");
			_abbreviations.Add("BF", "BURKINA FASO");
			_abbreviations.Add("BI", "BURUNDI");
			_abbreviations.Add("KH", "CAMBODIA");
			_abbreviations.Add("CM", "CAMEROON");
			_abbreviations.Add("CA", "CANADA");
			_abbreviations.Add("CV", "CAPE VERDE");
			_abbreviations.Add("KY", "CAYMAN ISLANDS");
			_abbreviations.Add("CF", "CENTRAL AFRICAN REPUBLIC");
			_abbreviations.Add("TD", "CHAD");
			_abbreviations.Add("CL", "CHILE");
			_abbreviations.Add("CN", "CHINA");
			_abbreviations.Add("CX", "CHRISTMAS ISLAND");
			_abbreviations.Add("CC", "COCOS (KEELING) ISLANDS");
			_abbreviations.Add("CO", "COLOMBIA");
			_abbreviations.Add("KM", "COMOROS");
			_abbreviations.Add("CG", "CONGO");
			_abbreviations.Add("CD", "CONGO THE DEMOCRATIC REPUBLIC OF THE");
			_abbreviations.Add("CK", "COOK ISLANDS");
			_abbreviations.Add("CR", "COSTA RICA");
			_abbreviations.Add("CI", "COTE D'IVOIRE");
			_abbreviations.Add("HR", "CROATIA");
			_abbreviations.Add("CU", "CUBA");
			_abbreviations.Add("CW", "CURACAO");
			_abbreviations.Add("CY", "CYPRUS");
			_abbreviations.Add("CZ", "CZECH REPUBLIC");
			_abbreviations.Add("DK", "DENMARK");
			_abbreviations.Add("DJ", "DJIBOUTI");
			_abbreviations.Add("DM", "DOMINICA");
			_abbreviations.Add("DO", "DOMINICAN REPUBLIC");
			_abbreviations.Add("EC", "ECUADOR");
			_abbreviations.Add("EG", "EGYPT");
			_abbreviations.Add("SV", "EL SALVADOR");
			_abbreviations.Add("GQ", "EQUATORIAL GUINEA");
			_abbreviations.Add("ER", "ERITREA");
			_abbreviations.Add("EE", "ESTONIA");
			_abbreviations.Add("ET", "ETHIOPIA");
			_abbreviations.Add("FK", "FALKLAND ISLANDS (MALVINAS)");
			_abbreviations.Add("FO", "FAROE ISLANDS");
			_abbreviations.Add("FJ", "FIJI");
			_abbreviations.Add("FI", "FINLAND");
			_abbreviations.Add("FR", "FRANCE");
			_abbreviations.Add("GF", "FRENCH GUIANA");
			_abbreviations.Add("PF", "FRENCH POLYNESIA");
			_abbreviations.Add("TF", "FRENCH SOUTHERN TERRITORIES");
			_abbreviations.Add("GA", "GABON");
			_abbreviations.Add("GM", "GAMBIA");
			_abbreviations.Add("GE", "GEORGIA");
			_abbreviations.Add("DE", "GERMANY");
			_abbreviations.Add("GH", "GHANA");
			_abbreviations.Add("GI", "GIBRALTAR");
			_abbreviations.Add("GR", "GREECE");
			_abbreviations.Add("GL", "GREENLAND");
			_abbreviations.Add("GD", "GRENADA");
			_abbreviations.Add("GP", "GUADELOUPE");
			_abbreviations.Add("GU", "GUAM");
			_abbreviations.Add("GT", "GUATEMALA");
			_abbreviations.Add("GG", "GUERNSEY");
			_abbreviations.Add("GN", "GUINEA");
			_abbreviations.Add("GW", "GUINEA-BISSAU");
			_abbreviations.Add("GY", "GUYANA");
			_abbreviations.Add("HT", "HAITI");
			_abbreviations.Add("HM", "HEARD ISLAND AND MCDONALD ISLANDS");
			_abbreviations.Add("VA", "HOLY SEE (VATICAN CITY STATE)");
			_abbreviations.Add("HN", "HONDURAS");
			_abbreviations.Add("HK", "HONG KONG");
			_abbreviations.Add("HU", "HUNGARY");
			_abbreviations.Add("IS", "ICELAND");
			_abbreviations.Add("IN", "INDIA");
			_abbreviations.Add("ID", "INDONESIA");
			_abbreviations.Add("IR", "IRAN ISLAMIC REPUBLIC OF");
			_abbreviations.Add("IQ", "IRAQ");
			_abbreviations.Add("IE", "IRELAND");
			_abbreviations.Add("IM", "ISLE OF MAN");
			_abbreviations.Add("IL", "ISRAEL");
			_abbreviations.Add("IT", "ITALY");
			_abbreviations.Add("JM", "JAMAICA");
			_abbreviations.Add("JP", "JAPAN");
			_abbreviations.Add("JE", "JERSEY");
			_abbreviations.Add("JO", "JORDAN");
			_abbreviations.Add("KZ", "KAZAKHSTAN");
			_abbreviations.Add("KE", "KENYA");
			_abbreviations.Add("KI", "KIRIBATI");
			_abbreviations.Add("KP", "KOREA DEMOCRATIC PEOPLE'S REPUBLIC OF");
			_abbreviations.Add("KR", "KOREA REPUBLIC OF");
			_abbreviations.Add("KW", "KUWAIT");
			_abbreviations.Add("KG", "KYRGYZSTAN");
			_abbreviations.Add("LA", "LAO PEOPLE'S DEMOCRATIC REPUBLIC");
			_abbreviations.Add("LV", "LATVIA");
			_abbreviations.Add("LB", "LEBANON");
			_abbreviations.Add("LS", "LESOTHO");
			_abbreviations.Add("LR", "LIBERIA");
			_abbreviations.Add("LY", "LIBYA");
			_abbreviations.Add("LI", "LIECHTENSTEIN");
			_abbreviations.Add("LT", "LITHUANIA");
			_abbreviations.Add("LU", "LUXEMBOURG");
			_abbreviations.Add("MO", "MACAO");
			_abbreviations.Add("MK", "MACEDONIA THE FORMER YUGOSLAV REPUBLIC OF");
			_abbreviations.Add("MG", "MADAGASCAR");
			_abbreviations.Add("MW", "MALAWI");
			_abbreviations.Add("MY", "MALAYSIA");
			_abbreviations.Add("MV", "MALDIVES");
			_abbreviations.Add("ML", "MALI");
			_abbreviations.Add("MT", "MALTA");
			_abbreviations.Add("MH", "MARSHALL ISLANDS");
			_abbreviations.Add("MQ", "MARTINIQUE");
			_abbreviations.Add("MR", "MAURITANIA");
			_abbreviations.Add("MU", "MAURITIUS");
			_abbreviations.Add("YT", "MAYOTTE");
			_abbreviations.Add("MX", "MEXICO");
			_abbreviations.Add("FM", "MICRONESIA FEDERATED STATES OF");
			_abbreviations.Add("MD", "MOLDOVA REPUBLIC OF");
			_abbreviations.Add("MC", "MONACO");
			_abbreviations.Add("MN", "MONGOLIA");
			_abbreviations.Add("ME", "MONTENEGRO");
			_abbreviations.Add("MS", "MONTSERRAT");
			_abbreviations.Add("MA", "MOROCCO");
			_abbreviations.Add("MZ", "MOZAMBIQUE");
			_abbreviations.Add("MM", "MYANMAR");
			_abbreviations.Add("NA", "NAMIBIA");
			_abbreviations.Add("NR", "NAURU");
			_abbreviations.Add("NP", "NEPAL");
			_abbreviations.Add("NL", "NETHERLANDS");
			_abbreviations.Add("NC", "NEW CALEDONIA");
			_abbreviations.Add("NZ", "NEW ZEALAND");
			_abbreviations.Add("NI", "NICARAGUA");
			_abbreviations.Add("NE", "NIGER");
			_abbreviations.Add("NG", "NIGERIA");
			_abbreviations.Add("NU", "NIUE");
			_abbreviations.Add("NF", "NORFOLK ISLAND");
			_abbreviations.Add("MP", "NORTHERN MARIANA ISLANDS");
			_abbreviations.Add("NO", "NORWAY");
			_abbreviations.Add("OM", "OMAN");
			_abbreviations.Add("PK", "PAKISTAN");
			_abbreviations.Add("PW", "PALAU");
			_abbreviations.Add("PS", "PALESTINE STATE OF");
			_abbreviations.Add("PA", "PANAMA");
			_abbreviations.Add("PG", "PAPUA NEW GUINEA");
			_abbreviations.Add("PY", "PARAGUAY");
			_abbreviations.Add("PE", "PERU");
			_abbreviations.Add("PH", "PHILIPPINES");
			_abbreviations.Add("PN", "PITCAIRN");
			_abbreviations.Add("PL", "POLAND");
			_abbreviations.Add("PT", "PORTUGAL");
			_abbreviations.Add("PR", "PUERTO RICO");
			_abbreviations.Add("QA", "QATAR");
			_abbreviations.Add("RE", "REUNION");
			_abbreviations.Add("RO", "ROMANIA");
			_abbreviations.Add("RU", "RUSSIAN FEDERATION");
			_abbreviations.Add("RW", "RWANDA");
			_abbreviations.Add("BL", "SAINT BARTHELEMY");
			_abbreviations.Add("SH", "SAINT HELENA ASCENSION AND TRISTAN DA CUNHA");
			_abbreviations.Add("KN", "SAINT KITTS AND NEVIS");
			_abbreviations.Add("LC", "SAINT LUCIA");
			_abbreviations.Add("MF", "SAINT MARTIN (FRENCH PART)");
			_abbreviations.Add("PM", "SAINT PIERRE AND MIQUELON");
			_abbreviations.Add("VC", "SAINT VINCENT AND THE GRENADINES");
			_abbreviations.Add("WS", "SAMOA");
			_abbreviations.Add("SM", "SAN MARINO");
			_abbreviations.Add("ST", "SAO TOME AND PRINCIPE");
			_abbreviations.Add("SA", "SAUDI ARABIA");
			_abbreviations.Add("SN", "SENEGAL");
			_abbreviations.Add("RS", "SERBIA");
			_abbreviations.Add("SC", "SEYCHELLES");
			_abbreviations.Add("SL", "SIERRA LEONE");
			_abbreviations.Add("SG", "SINGAPORE");
			_abbreviations.Add("SX", "SINT MAARTEN (DUTCH PART)");
			_abbreviations.Add("SK", "SLOVAKIA");
			_abbreviations.Add("SI", "SLOVENIA");
			_abbreviations.Add("SB", "SOLOMON ISLANDS");
			_abbreviations.Add("SO", "SOMALIA");
			_abbreviations.Add("ZA", "SOUTH AFRICA");
			_abbreviations.Add("GS", "SOUTH GEORGIA AND THE SOUTH SANDWICH ISLANDS");
			_abbreviations.Add("SS", "SOUTH SUDAN");
			_abbreviations.Add("ES", "SPAIN");
			_abbreviations.Add("LK", "SRI LANKA");
			_abbreviations.Add("SD", "SUDAN");
			_abbreviations.Add("SR", "SURINAME");
			_abbreviations.Add("SJ", "SVALBARD AND JAN MAYEN");
			_abbreviations.Add("SZ", "SWAZILAND");
			_abbreviations.Add("SE", "SWEDEN");
			_abbreviations.Add("CH", "SWITZERLAND");
			_abbreviations.Add("SY", "SYRIAN ARAB REPUBLIC");
			_abbreviations.Add("TW", "TAIWAN PROVINCE OF CHINA");
			_abbreviations.Add("TJ", "TAJIKISTAN");
			_abbreviations.Add("TZ", "TANZANIA UNITED REPUBLIC OF");
			_abbreviations.Add("TH", "THAILAND");
			_abbreviations.Add("TL", "TIMOR-LESTE");
			_abbreviations.Add("TG", "TOGO");
			_abbreviations.Add("TK", "TOKELAU");
			_abbreviations.Add("TO", "TONGA");
			_abbreviations.Add("TT", "TRINIDAD AND TOBAGO");
			_abbreviations.Add("TN", "TUNISIA");
			_abbreviations.Add("TR", "TURKEY");
			_abbreviations.Add("TM", "TURKMENISTAN");
			_abbreviations.Add("TC", "TURKS AND CAICOS ISLANDS");
			_abbreviations.Add("TV", "TUVALU");
			_abbreviations.Add("UG", "UGANDA");
			_abbreviations.Add("UA", "UKRAINE");
			_abbreviations.Add("AE", "UNITED ARAB EMIRATES");
			_abbreviations.Add("GB", "UNITED KINGDOM");
			_abbreviations.Add("US", "UNITED STATES");
			_abbreviations.Add("UM", "UNITED STATES MINOR OUTLYING ISLANDS");
			_abbreviations.Add("UY", "URUGUAY");
			_abbreviations.Add("UZ", "UZBEKISTAN");
			_abbreviations.Add("VU", "VANUATU");
			_abbreviations.Add("VE", "VENEZUELA BOLIVARIAN REPUBLIC OF");
			_abbreviations.Add("VN", "VIET NAM");
			_abbreviations.Add("VG", "VIRGIN ISLANDS BRITISH");
			_abbreviations.Add("VI", "VIRGIN ISLANDS U.S.");
			_abbreviations.Add("WF", "WALLIS AND FUTUNA");
			_abbreviations.Add("EH", "WESTERN SAHARA");
			_abbreviations.Add("YE", "YEMEN");
			_abbreviations.Add("ZM", "ZAMBIA");
			_abbreviations.Add("ZW", "ZIMBABWE");

			_countries = new Dictionary<string, string>(250);
			_countries.Add("AFGHANISTAN", "AF");
			_countries.Add("ALAND ISLANDS", "AX");
			_countries.Add("ALBANIA", "AL");
			_countries.Add("ALGERIA", "DZ");
			_countries.Add("AMERICAN SAMOA", "AS");
			_countries.Add("ANDORRA", "AD");
			_countries.Add("ANGOLA", "AO");
			_countries.Add("ANGUILLA", "AI");
			_countries.Add("ANTARCTICA", "AQ");
			_countries.Add("ANTIGUA AND BARBUDA", "AG");
			_countries.Add("ARGENTINA", "AR");
			_countries.Add("ARMENIA", "AM");
			_countries.Add("ARUBA", "AW");
			_countries.Add("AUSTRALIA", "AU");
			_countries.Add("AUSTRIA", "AT");
			_countries.Add("AZERBAIJAN", "AZ");
			_countries.Add("BAHAMAS", "BS");
			_countries.Add("BAHRAIN", "BH");
			_countries.Add("BANGLADESH", "BD");
			_countries.Add("BARBADOS", "BB");
			_countries.Add("BELARUS", "BY");
			_countries.Add("BELGIUM", "BE");
			_countries.Add("BELIZE", "BZ");
			_countries.Add("BENIN", "BJ");
			_countries.Add("BERMUDA", "BM");
			_countries.Add("BHUTAN", "BT");
			_countries.Add("BOLIVIA PLURINATIONAL STATE OF", "BO");
			_countries.Add("BONAIRE SINT EUSTATIUS AND SABA", "BQ");
			_countries.Add("BOSNIA AND HERZEGOVINA", "BA");
			_countries.Add("BOTSWANA", "BW");
			_countries.Add("BOUVET ISLAND", "BV");
			_countries.Add("BRAZIL", "BR");
			_countries.Add("BRITISH INDIAN OCEAN TERRITORY", "IO");
			_countries.Add("BRUNEI DARUSSALAM", "BN");
			_countries.Add("BULGARIA", "BG");
			_countries.Add("BURKINA FASO", "BF");
			_countries.Add("BURUNDI", "BI");
			_countries.Add("CAMBODIA", "KH");
			_countries.Add("CAMEROON", "CM");
			_countries.Add("CANADA", "CA");
			_countries.Add("CAPE VERDE", "CV");
			_countries.Add("CAYMAN ISLANDS", "KY");
			_countries.Add("CENTRAL AFRICAN REPUBLIC", "CF");
			_countries.Add("CHAD", "TD");
			_countries.Add("CHILE", "CL");
			_countries.Add("CHINA", "CN");
			_countries.Add("CHRISTMAS ISLAND", "CX");
			_countries.Add("COCOS (KEELING) ISLANDS", "CC");
			_countries.Add("COLOMBIA", "CO");
			_countries.Add("COMOROS", "KM");
			_countries.Add("CONGO", "CG");
			_countries.Add("CONGO THE DEMOCRATIC REPUBLIC OF THE", "CD");
			_countries.Add("COOK ISLANDS", "CK");
			_countries.Add("COSTA RICA", "CR");
			_countries.Add("COTE D'IVOIRE", "CI");
			_countries.Add("CROATIA", "HR");
			_countries.Add("CUBA", "CU");
			_countries.Add("CURACAO", "CW");
			_countries.Add("CYPRUS", "CY");
			_countries.Add("CZECH REPUBLIC", "CZ");
			_countries.Add("DENMARK", "DK");
			_countries.Add("DJIBOUTI", "DJ");
			_countries.Add("DOMINICA", "DM");
			_countries.Add("DOMINICAN REPUBLIC", "DO");
			_countries.Add("ECUADOR", "EC");
			_countries.Add("EGYPT", "EG");
			_countries.Add("EL SALVADOR", "SV");
			_countries.Add("EQUATORIAL GUINEA", "GQ");
			_countries.Add("ERITREA", "ER");
			_countries.Add("ESTONIA", "EE");
			_countries.Add("ETHIOPIA", "ET");
			_countries.Add("FALKLAND ISLANDS (MALVINAS)", "FK");
			_countries.Add("FAROE ISLANDS", "FO");
			_countries.Add("FIJI", "FJ");
			_countries.Add("FINLAND", "FI");
			_countries.Add("FRANCE", "FR");
			_countries.Add("FRENCH GUIANA", "GF");
			_countries.Add("FRENCH POLYNESIA", "PF");
			_countries.Add("FRENCH SOUTHERN TERRITORIES", "TF");
			_countries.Add("GABON", "GA");
			_countries.Add("GAMBIA", "GM");
			_countries.Add("GEORGIA", "GE");
			_countries.Add("GERMANY", "DE");
			_countries.Add("GHANA", "GH");
			_countries.Add("GIBRALTAR", "GI");
			_countries.Add("GREECE", "GR");
			_countries.Add("GREENLAND", "GL");
			_countries.Add("GRENADA", "GD");
			_countries.Add("GUADELOUPE", "GP");
			_countries.Add("GUAM", "GU");
			_countries.Add("GUATEMALA", "GT");
			_countries.Add("GUERNSEY", "GG");
			_countries.Add("GUINEA", "GN");
			_countries.Add("GUINEA-BISSAU", "GW");
			_countries.Add("GUYANA", "GY");
			_countries.Add("HAITI", "HT");
			_countries.Add("HEARD ISLAND AND MCDONALD ISLANDS", "HM");
			_countries.Add("HOLY SEE (VATICAN CITY STATE)", "VA");
			_countries.Add("HONDURAS", "HN");
			_countries.Add("HONG KONG", "HK");
			_countries.Add("HUNGARY", "HU");
			_countries.Add("ICELAND", "IS");
			_countries.Add("INDIA", "IN");
			_countries.Add("INDONESIA", "ID");
			_countries.Add("IRAN ISLAMIC REPUBLIC OF", "IR");
			_countries.Add("IRAQ", "IQ");
			_countries.Add("IRELAND", "IE");
			_countries.Add("ISLE OF MAN", "IM");
			_countries.Add("ISRAEL", "IL");
			_countries.Add("ITALY", "IT");
			_countries.Add("JAMAICA", "JM");
			_countries.Add("JAPAN", "JP");
			_countries.Add("JERSEY", "JE");
			_countries.Add("JORDAN", "JO");
			_countries.Add("KAZAKHSTAN", "KZ");
			_countries.Add("KENYA", "KE");
			_countries.Add("KIRIBATI", "KI");
			_countries.Add("KOREA DEMOCRATIC PEOPLE'S REPUBLIC OF", "KP");
			_countries.Add("KOREA REPUBLIC OF", "KR");
			_countries.Add("KUWAIT", "KW");
			_countries.Add("KYRGYZSTAN", "KG");
			_countries.Add("LAO PEOPLE'S DEMOCRATIC REPUBLIC", "LA");
			_countries.Add("LATVIA", "LV");
			_countries.Add("LEBANON", "LB");
			_countries.Add("LESOTHO", "LS");
			_countries.Add("LIBERIA", "LR");
			_countries.Add("LIBYA", "LY");
			_countries.Add("LIECHTENSTEIN", "LI");
			_countries.Add("LITHUANIA", "LT");
			_countries.Add("LUXEMBOURG", "LU");
			_countries.Add("MACAO", "MO");
			_countries.Add("MACEDONIA THE FORMER YUGOSLAV REPUBLIC OF", "MK");
			_countries.Add("MADAGASCAR", "MG");
			_countries.Add("MALAWI", "MW");
			_countries.Add("MALAYSIA", "MY");
			_countries.Add("MALDIVES", "MV");
			_countries.Add("MALI", "ML");
			_countries.Add("MALTA", "MT");
			_countries.Add("MARSHALL ISLANDS", "MH");
			_countries.Add("MARTINIQUE", "MQ");
			_countries.Add("MAURITANIA", "MR");
			_countries.Add("MAURITIUS", "MU");
			_countries.Add("MAYOTTE", "YT");
			_countries.Add("MEXICO", "MX");
			_countries.Add("MICRONESIA FEDERATED STATES OF", "FM");
			_countries.Add("MOLDOVA REPUBLIC OF", "MD");
			_countries.Add("MONACO", "MC");
			_countries.Add("MONGOLIA", "MN");
			_countries.Add("MONTENEGRO", "ME");
			_countries.Add("MONTSERRAT", "MS");
			_countries.Add("MOROCCO", "MA");
			_countries.Add("MOZAMBIQUE", "MZ");
			_countries.Add("MYANMAR", "MM");
			_countries.Add("NAMIBIA", "NA");
			_countries.Add("NAURU", "NR");
			_countries.Add("NEPAL", "NP");
			_countries.Add("NETHERLANDS", "NL");
			_countries.Add("NEW CALEDONIA", "NC");
			_countries.Add("NEW ZEALAND", "NZ");
			_countries.Add("NICARAGUA", "NI");
			_countries.Add("NIGER", "NE");
			_countries.Add("NIGERIA", "NG");
			_countries.Add("NIUE", "NU");
			_countries.Add("NORFOLK ISLAND", "NF");
			_countries.Add("NORTHERN MARIANA ISLANDS", "MP");
			_countries.Add("NORWAY", "NO");
			_countries.Add("OMAN", "OM");
			_countries.Add("PAKISTAN", "PK");
			_countries.Add("PALAU", "PW");
			_countries.Add("PALESTINE STATE OF", "PS");
			_countries.Add("PANAMA", "PA");
			_countries.Add("PAPUA NEW GUINEA", "PG");
			_countries.Add("PARAGUAY", "PY");
			_countries.Add("PERU", "PE");
			_countries.Add("PHILIPPINES", "PH");
			_countries.Add("PITCAIRN", "PN");
			_countries.Add("POLAND", "PL");
			_countries.Add("PORTUGAL", "PT");
			_countries.Add("PUERTO RICO", "PR");
			_countries.Add("QATAR", "QA");
			_countries.Add("REUNION", "RE");
			_countries.Add("ROMANIA", "RO");
			_countries.Add("RUSSIAN FEDERATION", "RU");
			_countries.Add("RWANDA", "RW");
			_countries.Add("SAINT BARTHELEMY", "BL");
			_countries.Add("SAINT HELENA ASCENSION AND TRISTAN DA CUNHA", "SH");
			_countries.Add("SAINT KITTS AND NEVIS", "KN");
			_countries.Add("SAINT LUCIA", "LC");
			_countries.Add("SAINT MARTIN (FRENCH PART)", "MF");
			_countries.Add("SAINT PIERRE AND MIQUELON", "PM");
			_countries.Add("SAINT VINCENT AND THE GRENADINES", "VC");
			_countries.Add("SAMOA", "WS");
			_countries.Add("SAN MARINO", "SM");
			_countries.Add("SAO TOME AND PRINCIPE", "ST");
			_countries.Add("SAUDI ARABIA", "SA");
			_countries.Add("SENEGAL", "SN");
			_countries.Add("SERBIA", "RS");
			_countries.Add("SEYCHELLES", "SC");
			_countries.Add("SIERRA LEONE", "SL");
			_countries.Add("SINGAPORE", "SG");
			_countries.Add("SINT MAARTEN (DUTCH PART)", "SX");
			_countries.Add("SLOVAKIA", "SK");
			_countries.Add("SLOVENIA", "SI");
			_countries.Add("SOLOMON ISLANDS", "SB");
			_countries.Add("SOMALIA", "SO");
			_countries.Add("SOUTH AFRICA", "ZA");
			_countries.Add("SOUTH GEORGIA AND THE SOUTH SANDWICH ISLANDS", "GS");
			_countries.Add("SOUTH SUDAN", "SS");
			_countries.Add("SPAIN", "ES");
			_countries.Add("SRI LANKA", "LK");
			_countries.Add("SUDAN", "SD");
			_countries.Add("SURINAME", "SR");
			_countries.Add("SVALBARD AND JAN MAYEN", "SJ");
			_countries.Add("SWAZILAND", "SZ");
			_countries.Add("SWEDEN", "SE");
			_countries.Add("SWITZERLAND", "CH");
			_countries.Add("SYRIAN ARAB REPUBLIC", "SY");
			_countries.Add("TAIWAN PROVINCE OF CHINA", "TW");
			_countries.Add("TAJIKISTAN", "TJ");
			_countries.Add("TANZANIA UNITED REPUBLIC OF", "TZ");
			_countries.Add("THAILAND", "TH");
			_countries.Add("TIMOR-LESTE", "TL");
			_countries.Add("TOGO", "TG");
			_countries.Add("TOKELAU", "TK");
			_countries.Add("TONGA", "TO");
			_countries.Add("TRINIDAD AND TOBAGO", "TT");
			_countries.Add("TUNISIA", "TN");
			_countries.Add("TURKEY", "TR");
			_countries.Add("TURKMENISTAN", "TM");
			_countries.Add("TURKS AND CAICOS ISLANDS", "TC");
			_countries.Add("TUVALU", "TV");
			_countries.Add("UGANDA", "UG");
			_countries.Add("UKRAINE", "UA");
			_countries.Add("UNITED ARAB EMIRATES", "AE");
			_countries.Add("UNITED KINGDOM", "GB");
			_countries.Add("UNITED STATES", "US");
			_countries.Add("UNITED STATES MINOR OUTLYING ISLANDS", "UM");
			_countries.Add("URUGUAY", "UY");
			_countries.Add("UZBEKISTAN", "UZ");
			_countries.Add("VANUATU", "VU");
			_countries.Add("VENEZUELA BOLIVARIAN REPUBLIC OF", "VE");
			_countries.Add("VIET NAM", "VN");
			_countries.Add("VIRGIN ISLANDS BRITISH", "VG");
			_countries.Add("VIRGIN ISLANDS U.S.", "VI");
			_countries.Add("WALLIS AND FUTUNA", "WF");
			_countries.Add("WESTERN SAHARA", "EH");
			_countries.Add("YEMEN", "YE");
			_countries.Add("ZAMBIA", "ZM");
			_countries.Add("ZIMBABWE", "ZW");
		}

		#endregion

		#region Public methods

		public static IList<string> GetAllCountries()
		{
			return new List<string>(_countries.Keys);
		}

		public static string GetTripleLanguageAbbreviation(string language, bool useUppercase = false)
		{
			string locale = "Eng";

			switch (language)
			{
				case "English":
					locale = "Eng";
					break;

				case "Spanish":
					locale = "Esp";
					break;

				case "Portuguese":
					locale = "Por";
					break;
			}

			locale = (useUppercase) ? locale.ToUpper() : locale;
			return locale;
		}

		public static string GetDoubleLanguageAbbreviation(string language, bool useUppercase = false)
		{
			string locale = "en";

			switch (language)
			{
				case "English":
					locale = "en";
					break;

				case "Spanish":
					locale = "es";
					break;

				case "Portuguese":
					locale = "pt";
					break;

				case "German":
					locale = "de";
					break;

				case "French":
					locale = "fr";
					break;

                case "Arabic":
                    locale = "ar";
                    break;

                case "Chinese":
                    locale = "zh";
                    break;
                
                case "Italian":
                    locale = "it";
                    break;

                case "Russian":
                    locale = "ru";
                    break;
			}

			locale = (useUppercase) ? locale.ToUpper() : locale;
			return locale;
		}

		public static string GetCountryAbbreviation(string country)
		{
			string upper = country.ToUpper();

			if (_countries.ContainsKey(upper))
				return _countries[upper];
			
			return "";
		}

		public static bool CheckAbbreviation(string abreviation)
		{
			return _abbreviations.ContainsKey(abreviation.ToUpper());
		}

		public static bool CheckCountry(string country)
		{
			return _countries.ContainsKey(country.ToUpper());
		}

		#endregion
	}
}