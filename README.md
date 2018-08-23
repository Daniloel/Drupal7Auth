# Drupal7Auth
auth of drupal 7 from xamarin app

## Drupal Services and Authentication

/api/system/connect.json  //For verify the users connected
/api/user/login.json // For log in the app
/api/user/logout,json //For logout of the app
/api/user/token.json //Get token when don't save in log in process and use for logout or connect


## Drupal headers

connect : token
logout: token, Cookie(sessid)
login: username,password
token : empty

## Drupal http

Need the HttpClient make the request and be the one instance for the cycle of log in, and logout.

if not use this, become an error of CSRF-Failure or user is no loged in or the connect api return anon users.
