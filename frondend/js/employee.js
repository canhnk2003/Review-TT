window.onload = function () {
  new EmployeePage();
};

class EmployeePage {
  pageTitle = "Quản lý nhân viên";
  inputInvalids = [];
  constructor() {
    this.initEvents();
    this.loadData();
  }
  /**
   * Khởi tạo các sự kiện trong page
   */
  initEvents() {
    try {
      var me = this;
      //Click button Thêm mới hiển thị form thêm mới
      const btnAdd = document.querySelector("#btnAdd");
      btnAdd.addEventListener("click", () => {
        this.btnAddOnClick();
        this.clearErrorMessages();
      });
      //Khi Click nút X (Close)
      const btnCloses = document.querySelectorAll(".m-dialog .m-dialog-close");
      for (const btnClose of btnCloses) {
        btnClose.addEventListener("click", function () {
          this.parentElement.parentElement.parentElement.style.visibility =
            "hidden";
          me.inputInvalids[0].focus();
        });
      }
      //Thêm focus vào ô input đầu tiên khi nhấn Đồng ý trong dialog báo lỗi
      document
        .querySelector("#dlgNotice .m-button")
        .addEventListener("click", function () {
          this.parentElement.parentElement.parentElement.style.visibility =
            "hidden";
          me.inputInvalids[0].focus();
        });
      //Khi click nút Hủy trong dialog detail
      document
        .querySelector(".m-dialog .m-button-secondary")
        .addEventListener("click", () => {
          document.querySelector(".m-dialog").style.visibility = "hidden";
        });
      //Refresh dữ liệu
      
      //Khi nhấn nút Cất
      document
        .querySelector("#btnSave")
        .addEventListener("click", this.btnSaveOnClick.bind(this));
      //Xuất file Excel

      //Khi nhấn nút radio button (Giới tính)
      const radios = document.querySelectorAll("#dlgDetail .m-radio-icon");
      let selectedRadio = null;
      for (const radio of radios) {
        radio.addEventListener("click", function (radio) {
          //Bỏ chọn radio hiện tại nếu có
          if (selectedRadio !== null) {
            selectedRadio.classList.remove("m-radio-icon-checked");
            selectedRadio.classList.add("m-radio-icon-unchecked");
          }
          //Chọn radio mới
          this.classList.remove("m-radio-icon-unchecked");
          this.classList.add("m-radio-icon-checked");
          selectedRadio = this;
        });
      }
      //Nhấn nút edit thì hiển thị dialog sửa
      const btnEdits = document.querySelectorAll("table .m-edit");
      for (const btnEdit of btnEdits) {
        btnEdit.addEventListener("click", () => {
          this.btnEditOnClick();
          this.clearErrorMessages();
        });
      }
    } catch (error) {
      console.error(error);
    }
  }

  loadData() {
    try {
      //Gọi API lấy dữ liệu
      fetch("https://cukcuk.manhnv.net/api/v1/Employees")
        .then((res) => res.json())
        .then((data) => {
          //Lấy ra table
          const table = document.querySelector("#tblEmployee");
          //Duyệt từng phần tử trong data
          let stt = 1;
          for (const item of data) {
            let tr = document.createElement("tr");
            // Chuyển đổi item.DateOfBirth từ chuỗi thành đối tượng Date (nếu cần)
            let dateOfBirth = new Date(item.DateOfBirth);

            // Lấy thông tin ngày, tháng, năm từ đối tượng Date
            let day = dateOfBirth.getDate();
            let month = dateOfBirth.getMonth() + 1; // Tháng đếm từ 0, cộng thêm 1 để đúng tháng
            let year = dateOfBirth.getFullYear();
            // Định dạng lại ngày tháng theo dd/MM/yyyy
            let formattedDateOfBirth = `${day}/${month}/${year}`;
            tr.innerHTML = `<td class="m-data-center"><input type="checkbox" /></td>
                            <td>${stt}</td>
                            <td empIdCell>${item.EmployeeCode}</td>
                            <td>${item.FullName}</td>
                            <td>${item.GenderName}</td>
                            <td>${formattedDateOfBirth}</td>
                            <td>${item.Email}</td>
                            <td>${item.Address}</td>
                            <td>
                              <div class="m-table-tool">
                                <div class="m-edit m-tool-icon"></div>
                                <div class="m-more m-tool-icon"></div>
                              </div>
                            </td>`;
            table.querySelector("tbody").append(tr);
            stt++;
          }
        });
    } catch (error) {
      console.error(error);
    }
  }
  /**
   * Click vào button Thêm mới
   */
  btnAddOnClick() {
    try {
      //Hiển thị form Thêm mới
      //1. Lấy ra element của form thêm mới
      const dlgDetail = document.querySelector("#dlgDetail");
      //2. Set hiển thị form
      dlgDetail.style.visibility = "visible";
      dlgDetail.querySelector("#empId").focus();
    } catch (error) {
      console.error(error);
    }
  }
  btnSaveOnClick() {
    try {
      //1. Thực hiện kiểm tra dữ liệu hợp lệ
      const validateRequired = this.checkRequiredInput();
      //2. Đưa ra dialog thông báo nếu lỗi
      if (validateRequired.errors.length > 0) {
        let dlgNotice = document.querySelector("#dlgNotice");
        //Cho dialog hiện lên
        dlgNotice.style.visibility = "visible";
        //Đổi tên tiêu dề cho dialog
        dlgNotice.querySelector(".m-dialog-title").innerHTML =
          "Dữ liệu không hợp lệ";
        //Xóa hết nội dung cũ
        let errorElement = dlgNotice.querySelector(".m-dialog-row");
        errorElement.innerHTML = "";
        //Thêm thông báo lỗi
        errorElement.style.display = "block";
        for (const msg of validateRequired.errors) {
          const m = msg.replace(/\*/g, "");
          let li = document.createElement("li");
          li.textContent = m;
          errorElement.append(li);
        }
        //Focus vào ô input lỗi đầu tiên
        this.inputInvalids = validateRequired.inputInvalid;
      } else {
        //
      }
    } catch (error) {
      console.error(error);
    }
  }
  addErrorElementToInputInvalid(input) {
    try {
      const nextElement = input.nextElementSibling;
      if (
        !input.classList.contains("m-input-error") ||
        !nextElement ||
        nextElement.tagName.toLowerCase() !== "div"
      ) {
        //Thêm border màu đỏ
        input.classList.add("m-input-error");
        //Bổ sung thông tin lỗi
        let errorElementMsg = document.createElement("div");
        errorElementMsg.classList.add("m-input-error-msg");
        errorElementMsg.textContent = "Thông tin này không được để trống!";
        input.after(errorElementMsg);
      }
    } catch (error) {
      console.log(error);
    }
  }
  checkRequiredInput() {
    try {
      let result = {
        inputInvalid: [],
        errors: [],
      };
      //Lấy ra các input bắt buộc nhập
      let inputs = document.querySelectorAll("#dlgDetail input[required]");
      for (const input of inputs) {
        const value = input.value;
        const nextElement = input.nextElementSibling;
        if (value === "" || value === null || value === undefined) {
          const lable = input.previousElementSibling;
          this.addErrorElementToInputInvalid(input);
          result.inputInvalid.push(input);
          result.errors.push(`${lable.textContent} không được để trống!`);
        } else if (
          input.classList.contains("m-input-error") ||
          nextElement ||
          nextElement.tagName.toLowerCase() === "div"
        ) {
          input.classList.remove("m-input-error");
          input.nextElementSibling.remove();
          // const errorMessages = document.querySelectorAll(
          //   "#dlgDetail .m-input-error-msg"
          // );
          //errorMessages.forEach((errorMessage) => errorMessage.remove());
        }
      }
      return result;
    } catch (error) {
      console.log(error);
    }
  }
  clearErrorMessages() {
    try {
      const dlgDetail = document.querySelector("#dlgDetail");
      const errorMessages = dlgDetail.querySelectorAll(".m-input-error-msg");
      const inputErrors = dlgDetail.querySelectorAll("input");
      errorMessages.forEach((errorMessage) => errorMessage.remove());
      for (const inputError of inputErrors) {
        inputError.classList.remove("m-input-error");
      }
    } catch (error) {
      console.log(error);
    }
  }
  btnEditOnClick() {
    try {
      let dlgEdit = document.querySelector("#dlgDetail");
      //Hiện ô dialog
      dlgEdit.style.visibility = "visible";
      //Đổi tên tiêu đề dialog
      dlgEdit.querySelector(".m-dialog-title").innerHTML =
        "Sửa thông tin nhân viên";
      dlgDetail.querySelector("#empId").focus();
    } catch (error) {
      console.log(error);
    }
  }
}
