function receiveData() {
  if(vm.tempA1 < 1000) {
    vm.tempA1 = vm.tempA1 + 1;
  }
  else {
    vm.temA = 0.4;
  }
  var d = new Date();
  vm.dt = d.toLocaleString();
}
setInterval(receiveData, 1000);
