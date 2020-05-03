import Home from './components/home';

import './index.scss';

let home = new Home();
document.getElementById("app").innerHTML = home.getHtml();