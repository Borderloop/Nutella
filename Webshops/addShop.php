<link rel="stylesheet" href="css/stylesheetMAIN.css" type="text/css">
    <head><meta http-equiv="Content-Type" content="text/html; charset=utf-8" /></head>
    <h2>Voeg een webshop toe<span style="color:#f05a23">!</span></h2>

    <form method="post" action="">   
        <div class="col_1">
          <div class="subcol_1"><b>Webshop naam</b><br/><input name="name" placeholder="Webshop Naam" type="text" required></div>
          <div class="subcol_1"><b>Webshop URL</b><br/><input name="url" placeholder="Webshop url" type="text" required></div>
          <div class="subcol_1"><b>Webshop Logo Groot</b><br/><input name="logoLarge" placeholder="Locatie Logo Groot" type="text"></div>
        </div>

        <div class="col_1"><b>Opereert in landen</b><br/>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="nl">Nederland</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="be">Belgi&euml;</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="de">Duitsland</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="uk">Verenigd Koninkrijk</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="fr">Frankrijk</input></div>

          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="it">Itali&euml;</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="es">Spanje</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="pt">Portugal</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="se">Zweden</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="no">Noorwegen</input></div>

          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="dk">Denemarken</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="pl">Polen</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="at">Oostenrijk</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="ch">Zwitserland</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="ie">Ierland</input></div>

          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="us">Verenigde Staten</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="cn">China</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="jp">Japan</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="fi">Finland</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="ru">Rusland</input></div>
          <div class="option_1" ><input type="checkbox" input name="operatingCountry[]" value="eu">Europa</input></div>
        </div>

        <div class="col_1"><b>Talen uitgevoerd</b><br/>
          <div class="option_1" ><input type="checkbox" input name="languages[]" value="nl">Nederlands</input></div>
          <div class="option_1" ><input type="checkbox" input name="languages[]" value="be">Belgisch</input></div>
          <div class="option_1" ><input type="checkbox" input name="languages[]" value="de">Duits</input></div>
          <div class="option_1" ><input type="checkbox" input name="languages[]" value="uk">Engels</input></div>

          <div class="option_1" ><input type="checkbox" input name="languages[]" value="fr">Frans</input></div>
          <div class="option_1" ><input type="checkbox" input name="languages[]" value="it">Italiaans</input></div>
          <div class="option_1" ><input type="checkbox" input name="languages[]" value="es">Spaans</input></div>
          <div class="option_1" ><input type="checkbox" input name="languages[]" value="pt">Portugees</input></div>

          <div class="option_1" ><input type="checkbox" input name="languages[]" value="se">Zweeds</input></div>
          <div class="option_1" ><input type="checkbox" input name="languages[]" value="no">Noors</input></div>
          <div class="option_1" ><input type="checkbox" input name="languages[]" value="dk">Deens</input></div>
          <div class="option_1" ><input type="checkbox" input name="languages[]" value="pl">Pools</input></div>

          <div class="option_1" ><input type="checkbox" input name="languages[]" value="ie">Iers</input></div>
          <div class="option_1" ><input type="checkbox" input name="languages[]" value="cn">Chinees</input></div>
          <div class="option_1" ><input type="checkbox" input name="languages[]" value="jp">Japans</input></div>
          <div class="option_1" ><input type="checkbox" input name="languages[]" value="fi">Fins</input></div>
          <div class="option_1" ><input type="checkbox" input name="languages[]" value="ru">Russisch</input></div>
        </div>

        <div class="col_1"><b>Post order bedrijf</b><br/>
          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="PostNL">PostNL</input></div>
          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="DHL">DHL</input></div>
          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="DPD">DPD</input></div>
          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="UPS">UPS</input></div>

          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="Chronopost">Chronopost</input></div>
          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="GLS">GLS</input></div>
          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="Selektvracht">Selektvracht</input></div>
          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="Colissimo">Colissimo</input></div>

          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="Royal Mail">Royal Mail</input></div>
          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="AT POST">AT Post</input></div>
          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="TNT">TNT</input></div>
          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="Hermes">Hermes</input></div>

          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="iParcel">iParcel</input></div>
          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="FedEx">FedEx</input></div>
          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="B-Post">B-Post</input></div>
          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="Dynalogic">Dynalogic</input></div>
          <div class="option_1" ><input type="checkbox" input name="shippingCompanies[]" value="Bartolini">Bartolini</input></div>
        </div>

        <div class="col_1"><b>Verzendkosten</b>
          <input name="shippingCost" placeholder="&#8364;25,-" type="text"></div>
        </div>

        <div class="col_1"><b>Verzend naar landen</b><br/>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="eu">Europa</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="ww">Wereldwijd</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="nl">Nederland</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="be">Belgi&euml;</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="de">Duitsland</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="uk">Verenigd Koninkrijk</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="fr">Frankrijk</input></div>

          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="it">Itali&euml;</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="es">Spanje</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="pt">Portugal</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="se">Zweden</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="no">Noorwegen</input></div>

          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="dk">Denemarken</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="pl">Polen</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="at">Oostenrijk</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="ch">Zwitserland</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]t" value="ie">Ierland</input></div>

          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="us">Verenigde Staten</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="cn">China</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="jp">Japan</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="fi">Finland</input></div>
          <div class="option_1" ><input type="checkbox" input name="sendingCountry[]" value="ru">Rusland</input></div>
        </div>

        <div class="col_1"><b>Betaalmethodes naar NL</b><br/>
          <div class="option_1" ><input type="checkbox" input name="paymentMethods[]" value="Vooraf betalen">Vooraf betalen</input></div>
          <div class="option_1" ><input type="checkbox" input name="paymentMethods[]" value="iDeal">iDeal</input></div>
          <div class="option_1" ><input type="checkbox" input name="paymentMethods[]" value="Mastercard">Mastercard</input></div>
          <div class="option_1" ><input type="checkbox" input name="paymentMethods[]" value="PayPal">PayPal</input></div>

          <div class="option_1" ><input type="checkbox" input name="paymentMethods[]" value="Visa">Visa</input></div>
          <div class="option_1" ><input type="checkbox" input name="paymentMethods[]" value="Sofort">Sofort</input></div>
          <div class="option_1" ><input type="checkbox" input name="paymentMethods[]" value="Amex">Amex</input></div>
          <div class="option_1" ><input type="checkbox" input name="paymentMethods[]" value="Rembours">Rembours</input></div>

          <div class="option_1" ><input type="checkbox" input name="paymentMethods[]" value="Maestro">Maestro</input></div>
          <div class="option_1" ><input type="checkbox" input name="paymentMethods[]" value="Mister Cash">Mister Cash</input></div>
          <div class="option_1" ><input type="checkbox" input name="paymentMethods[]" value="TNT">TNT</input></div>
          <div class="option_1" ><input type="checkbox" input name="paymentMethods[]" value="Afterpay">Afterpay</input></div>

          <div class="option_1" ><input type="checkbox" input name="paymentMethods[]" value="Klarna">Klarna</input></div>
          <div class="option_1" ><input type="checkbox" input name="paymentMethods[]" value="Bitcoin">Bitcoin</input></div>
        </div>

        <div class="col_1"><b>Keurmerken</b><br/>
          <div class="option_1" ><input type="checkbox" name="mark_list[]" value="-">Geen</input></div>
          <div class="option_1" ><input type="checkbox" name="mark_list[]" value="Thuiswinkel">Thuiswinkel</input></div>
          <div class="option_1" ><input type="checkbox" name="mark_list[]" value="Chip Online">Chip Online</input></div>
          <div class="option_1" ><input type="checkbox" name="mark_list[]" value="Digikeur">Digikeur</input></div>
          <div class="option_1" ><input type="checkbox" name="mark_list[]" value="eKomi">eKomi</input></div>
          <div class="option_1" ><input type="checkbox" name="mark_list[]" value="EHI">EHI</input></div>
          <div class="option_1" ><input type="checkbox" name="mark_list[]" value="Euro Label">Euro Label</input></div>
          <div class="option_1" ><input type="checkbox" name="mark_list[]" value="Euro Safe Shop">Euro Safe Shop</input></div>
          <div class="option_1" ><input type="checkbox" name="mark_list[]" value="FIA-net">FIA-net</input></div>
          <div class="option_1" ><input type="checkbox" name="mark_list[]" value="ICT Waarborg">ICT Waarborg</input></div>          
          <div class="option_1" ><input type="checkbox" name="mark_list[]" value="Q-Shops">Q-Shops</input></div>
          <div class="option_1" ><input type="checkbox" name="mark_list[]" value="Trusted Shops E-Guarantee">Trusted Shops</input></div>
          <div class="option_1" ><input type="checkbox" name="mark_list[]" value="TUV">TUV</input></div>
          <div class="option_1" ><input type="checkbox" name="mark_list[]" value="Webshop Keurmerk">Webshop Keurmerk</input></div>
        </div>

        <div class="col_1">
          <div class="subcol_1"><b>Affiliate</b>
            <div class="option_1" >Client Partner Program<input type="checkbox" name="affiliate_list[]" value="Client Partner Program"></div>
            <div class="option_1" >Betsy<input type="checkbox" name="affiliate_list[]" value="Betsy"></div>
            <div class="option_1" >Zanox<input type="checkbox" name="affiliate_list[]" value="Zanox"></div>
            <div class="option_1" >Belboon<input type="checkbox" name="affiliate_list[]" value="Belboon"></div>
            <div class="option_1" >Tradetracker<input type="checkbox" name="affiliate_list[]" value="Tradetracker"></div>
          </div>
          <div class="subcol_1"><b>Affiliate Status</b>            
            <div class="option_1"><input type="radio" input name="affApproved" value="1">Approved</input></div>
            <div class="option_1"><input type="radio" input name="affApproved" value="2">Pending</input></div>
            <div class="option_1"><input type="radio" input name="affApproved" value="3">Denied</input></div>
          </div>
        </div>
      <br/><br/><br/><br/>
      <input class="contact-button" name="submit" type="submit" value="INSERT">
    </form>
</div>

<?php

require_once('db_handler.php');
$conn = iniConn(); // Returns the opened database connection

$countryArray = array(
  "nl"=>"Nederland",    "be"=>"België",
  "de"=>"Duitsland",    "uk"=>"Verenigd Koninkrijk",
  "fr"=>"Frankrijk",    "it"=>"Italië",
  "es"=>"Spanje",       "pt"=>"Portugal",
  "se"=>"Zweden",       "no"=>"Noorwegen",
  "dk"=>"Denemarken",   "pl"=>"Polen",
  "at"=>"Oostenrijk",   "ch"=>"Zwitserland",
  "ie"=>"Ierland",      "us"=>"Verenigde Staten",
  "cn"=>"China",        "jp"=>"Japan",
  "fi"=>"Finland",      "ru"=>"Rusland",
  "eu"=>"Europa", 
  );

if($_SERVER["REQUEST_METHOD"] == "POST"){
  // If the submit button is pressed the form data is loaded into variables.
  $wsName = $_POST['name'];
  $wsUrl = $_POST['url'];
  $wsAffApproved = 0; 
  $wsShippingCost = null; 
  $wsLogoLarge = $_POST['logoLarge'];

  //With the isset function we check if the form data is not null, and then proceed to use that data
  if(isset($_POST['affApproved'])){
    $wsAffApproved = $_POST['affApproved']; // Loads data into variable
  }
  if(isset($_POST['shippingCost']) & !empty($_POST['shippingCost'])){
    $wsShippingCost = currencyParser($_POST['shippingCost']); // Converts to decimal value
  }
  
  // A statement is prepared with unassigned parameters, with the execute function we run the previously made statement and bind parameters to make it work.
  // The reason we do this is because if a person with malicious intent reaches this part of the website they will not be able to do an SQL injection.
  $stmt = $conn->prepare("INSERT INTO webshop(name, url, logo_large, shipping_cost)VALUES(:wsName, :wsUrl, :wsLogoLarge, :wsShippingCost)");
  $stmt->execute(array(
    "wsName" => $wsName,
    "wsUrl" => $wsUrl,
    ":wsLogoLarge"=> $wsLogoLarge,
    "wsShippingCost" => $wsShippingCost
  ));
  $wsID = $conn->lastInsertId(); // We save the id of the last inputted webshop to use it with the other inserts.

  if(isset($wsID)){ 

    if(isset($_POST['operatingCountry'])){
      foreach($_POST['operatingCountry'] as $country){ // We loop through an array of checkbox values and insert every value to the previously obtained webshop_id
        $stmt = $conn->prepare("INSERT INTO operatingcountry (country, webshop_id) VALUES (:wsCountry,:wsId)");
        $stmt->execute(array(
          "wsCountry" => $countryArray[$country],
          "wsId" => $wsID
          ));
      }
    }

    if(isset($_POST['languages'])){
      foreach($_POST['languages'] as $language){ // We loop through an array of checkbox values and insert every value to the previously obtained webshop_id
        $stmt = $conn->prepare("INSERT INTO language (language,webshop_id) VALUES (:wsLanguage,:wsId)");
        $stmt->execute(array(
          "wsLanguage" => strtoupper($language),
          "wsId" => $wsID
          ));
      }
    }
  
    if(isset($_POST['shippingCompanies'])){ 
      foreach($_POST['shippingCompanies'] as $shippingcompany){ // We loop through an array of checkbox values and insert every value to the previously obtained webshop_id
        $stmt = $conn->prepare("INSERT INTO sender (sender, webshop_id) VALUES (:wsSender,:wsId)");
        $stmt->execute(array(
          "wsSender" => $shippingcompany,
          "wsId" => $wsID
          ));
      }
    }

    if(isset($_POST['sendingCountry'])){
      foreach($_POST['sendingCountry'] as $sendingcountry){ // We loop through an array of checkbox values and insert every value to the previously obtained webshop_id
        $stmt = $conn->prepare("INSERT INTO sendingcountry (country, webshop_id) VALUES (:wsCountry,:wsId)");
        $stmt->execute(array(
          "wsCountry" => strtoupper($sendingcountry),
          "wsId" => $wsID
          ));
      }
    }

    if(isset($_POST['paymentMethods'])){
      foreach($_POST['paymentMethods'] as $paymentmethod){ // We loop through an array of checkbox values and insert every value to the previously obtained webshop_id
        $stmt = $conn->prepare("INSERT INTO payment_method (method, webshop_id) VALUES (:wsMethod,:wsId)");
        $stmt->execute(array(
          "wsMethod" => $paymentmethod,
          "wsId" => $wsID
          ));
      }
    }

    if(isset($_POST['mark_list'])){
      foreach($_POST['mark_list'] as $mark){ // We loop through an array of checkbox values and insert every value to the previously obtained webshop_id
        $stmt = $conn->prepare("INSERT INTO mark (mark, webshop_id) VALUES (:wsMark,:wsId)");
        $stmt->execute(array(
          "wsMark" => $mark,
          "wsId" => $wsID
          ));
      }
    }

    if(isset($_POST['affiliate_list'])){
      foreach($_POST['affiliate_list'] as $affiliate){ // We loop through an array of checkbox values and insert every value to the previously obtained webshop_id
        $stmt = $conn->prepare("INSERT INTO affiliate (affiliate, approved, webshop_id) VALUES (:affiliate,:wsAffApproved,:wsId)");
        $stmt->execute(array(
          "affiliate"=>$affiliate,
          "wsAffApproved"=>$wsAffApproved,
          "wsId"=>$wsID
          ));
      }
    }
  }
}

// The currency parser function checks what the given price and currency is, after that it will be converted to euro's and returned to insert in the database.
function currencyParser($stringPrice){
  $stringPrice = htmlentities($stringPrice);
  $stringPrice = str_replace(',','.',$stringPrice); // The database does not recognize , so they will be replaced with .
  if (!is_numeric(substr($stringPrice,0,1))){ // If the first value is not a number it will be a dollar/euro/pound sign
      if(substr($stringPrice,0,1)==="$"){ // If the first value is a dollar we put the dollar sign in a variable. The dollar sign is ASCII and not UTF-8 hence the exception 'if'
        $currency = substr($stringPrice,0,1);
        $oldPrice = substr($stringPrice,1,strlen($stringPrice)); // We put the old price in dollars into a variable
        $newPrice = floatval($oldPrice*0.773029); // The new price is calculated
        return $newPrice; // The price in euro's is returned
      }
      else{
        $currency = preg_split('/(;)/',$stringPrice,null,PREG_SPLIT_DELIM_CAPTURE); // The other currency symbols are not ascii so will have to be split based on ;
        $currency = $currency[0].$currency[1]; // We put the whole currency symbol in one variable and then check which one it is. Based on that we convert it to euro's and return it.
        switch ($currency){
          case("&pound;"):
              $oldPrice = substr($stringPrice,strlen($currency));
              $newPrice = floatval($oldPrice*1.25320);
              return $newPrice;
          case("&euro;"):
              $oldPrice = substr($stringPrice,strlen($currency));
              $newPrice = floatval($oldPrice);
              return $newPrice;
        }
      }
      
  }else{
    return floatval($stringPrice); // If a currency is not given we will assume the price was given in euro's, i.e. 24,-
  }
}

  $conn = null; // Closes the database connection
?>