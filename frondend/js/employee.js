window.onload = function () {
  new EmployeePage();
};

class EmployeePage {
  pageTitle = "Quản lý nhân viên";
  inputInvalids = [];
  formMode = "add";
  currentPage = 1;
  employeeIdForEdit = "";
  employeeIdForDelete = "";
  employeesData = []; // Mảng này dùng để lưu trữ các nhân viên lấy từ API về
  constructor() {
    this.initEvents();
    this.fetchData();
  }
  /**
   * Khởi tạo các sự kiện trong page
   */
  initEvents() {
    try {
      var me = this;

      //Hiển thị danh sách phòng ban từ API vào select, option
      this.fetchDepartmentsAndPopulateSelect();

      //Click button Thêm mới hiển thị form thêm mới
      const btnAdd = document.querySelector("#btnAdd");
      btnAdd.addEventListener("click", () => {
        this.formMode="add";
        this.btnAddOnClick();
        this.clearErrorMessages();
      });

      //Khi Click nút X (Close)
      const btnCloses = document.querySelectorAll(".m-dialog .m-dialog-close");
      for (const btnClose of btnCloses) {
        btnClose.addEventListener("click", function () {
          this.parentElement.parentElement.parentElement.style.visibility =
            "hidden";
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
      document
        .querySelector(".m-toolbar-refresh")
        .addEventListener("click", () => {
          this.btnRefreshOnclick();
          this.fetchData();
        });

      //Khi nhấn nút Cất trong dialog chi tiết
      document
        .querySelector("#btnSave")
        .addEventListener("click", this.btnSaveOnClick.bind(this));

      //Xuất file Excel
      document
        .querySelector(".m-toolbar-table .m-toolbar-export")
        .addEventListener("click", this.exportDataToExcel.bind(this));

      //Khi nhấn nút radio button (Giới tính) trong dialog
      const radios = document.querySelectorAll("#dlgDetail .m-radio-icon");

      radios.forEach(function (radio) {
        radio.addEventListener("click", function () {
          // Loại bỏ class m-radio-icon-checked từ tất cả các radio
          radios.forEach(function (r) {
            r.classList.remove("m-radio-icon-checked");
            r.classList.add("m-radio-icon-unchecked");
          });

          // Thêm class m-radio-icon-checked cho radio được click
          this.classList.remove("m-radio-icon-unchecked");
          this.classList.add("m-radio-icon-checked");
        });
      });

      //Nhấn nút edit thì hiển thị dialog sửa
      document.querySelector("table").addEventListener("click", (event) => {
        if (event.target.classList.contains("m-edit")) {
          this.formMode = "edit";
          // Lấy ra phần tử mà người dùng đã nhấn
          let btnEdit = event.target;
          let trElement = btnEdit.parentElement.parentElement.closest("tr");
          const employeeId =
            trElement.querySelector("td[empIdCell]").textContent;
          this.employeeIdForEdit = employeeId;
          // Hiển thị dialog sửa
          this.btnEditOnClick();
          //Xóa dữ liệu cũ trong dialog
          this.clearErrorMessages();
          //Binding dữ liệu vào các ô input
          this.getDataToEditEmployee(employeeId);
        }
      });

      //Nhấn nút delete thì hiển thị dialog xác nhận xóa
      document.querySelector("table").addEventListener("click", (event) => {
        if (event.target.classList.contains("m-delete")) {
          this.formMode = "delete";
          // Lấy ra phần tử mà người dùng đã nhấn
          let btnDelete = event.target;
          let trElement = btnDelete.parentElement.parentElement.closest("tr");
          const employeeId =
            trElement.querySelector("td[empIdCell]").textContent;
          this.employeeIdForDelete = employeeId;
          // Hiển thị dialog sửa
          this.btnDeleteOnClick(employeeId);
        }
      });

      //Xử lý sự kiện khi nhấn nút checkbox
      this.btnCheckboxOnClick();

      //Xử lý sự kiện cho nút tìm kiếm (filter)
      this.filterData();

      //Xử lý phân trang cho dữ liệu bảng
      this.paginationForData();
    } catch (error) {
      console.error(error);
    }
  }
  /**
   * Xử lý sự kiện cho nút tìm kiếm (filter)
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  filterData() {
    const searchInput = document.querySelector(".m-input-search");
    searchInput.addEventListener("input", () => {
      const searchTerm = searchInput.value.trim().toLowerCase();
      const filteredData = this.employeesData.filter(
        (item) =>
          item.EmployeeCode.toLowerCase().includes(searchTerm) ||
          item.FullName.toLowerCase().includes(searchTerm)
      );
      this.btnRefreshOnclick();
      this.loadData(filteredData);
    });
  }
  /**
   * Xử lý sự kiện khi checkAll được nhấn và các checkbox trong body được nhấn
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  btnCheckboxOnClick() {
    const tableBody = document.querySelector("#tblEmployee tbody");
    const checkAll = document.getElementById("check-all");
    //Xử lý sự kiện khi checkAll được nhấn
    checkAll.addEventListener("change", () => {
      const checkboxes = tableBody.querySelectorAll('input[type="checkbox"]');
      checkboxes.forEach((checkbox) => {
        checkbox.checked = checkAll.checked;
        this.toggleRowClass(checkbox);
      });
    });

    // Xử lý sự kiện khi các checkbox trong body được nhấn
    tableBody.addEventListener("change", (event) => {
      if (event.target.matches('input[type="checkbox"]')) {
        this.toggleRowClass(event.target);
        // Kiểm tra xem tất cả các checkbox có được chọn hay không để chọn checkAll
        checkAll.checked = [
          ...tableBody.querySelectorAll('input[type="checkbox"]'),
        ].every((c) => c.checked);
      }
    });
  }
  /**
   * Thêm class vào tr khi checkbox được click
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  toggleRowClass(checkbox) {
    const row = checkbox.closest("tr");
    if (checkbox.checked) {
      row.classList.add("m-data-table-selected");
    } else {
      row.classList.remove("m-data-table-selected");
    }
  }
  /**
   * Tải lại dữ liệu từ API vào bảng
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  fetchData() {
    try {
      let loading = document.querySelector(".m-loading");
      loading.style.visibility = "visible";
      //Gọi API lấy dữ liệu
      fetch("https://cukcuk.manhnv.net/api/v1/Employees")
        .then((res) => res.json())
        .then((data) => {
          this.employeesData = data;
          this.loadData(data);
          loading.style.visibility = "hidden";
        });
    } catch (error) {
      console.error(error);
    }
  }
  /**
   * Tải lại dữ liệu từ API vào bảng
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  loadData(data) {
    const pagination = document.querySelector(".m-pagination");
    const recordsPerPageSelect = pagination.querySelector("#recordsPerPage");
    let recordsPerPage = parseInt(recordsPerPageSelect.value, 10);
    const startIndex = (this.currentPage - 1) * recordsPerPage;
    const endIndex = startIndex + recordsPerPage;
    const paginatedEmployees = data.slice(startIndex, endIndex);
    if (paginatedEmployees.length === 0 && this.currentPage > 1) {
      this.currentPage--; // Đảm bảo không bị lặp lại trang trống
      this.loadData(data); // Render lại bảng với trang trước đó
      return;
    }
    //Lấy ra table
    const table = document.querySelector("#tblEmployee");
    //Duyệt từng phần tử trong data
    let stt = 1;
    for (const item of paginatedEmployees) {
      let tr = document.createElement("tr");
      // Chuyển đổi item.DateOfBirth từ chuỗi thành đối tượng Date (nếu cần)
      let dateOfBirth = new Date(item.DateOfBirth);

      // Lấy thông tin ngày, tháng, năm từ đối tượng Date
      let day = dateOfBirth.getDate();
      let month = dateOfBirth.getMonth() + 1; // Tháng đếm từ 0, cộng thêm 1 để đúng tháng
      let year = dateOfBirth.getFullYear();
      // Định dạng lại ngày tháng theo dd/MM/yyyy
      day = day < 10 ? `0${day}` : day;
      month = month < 10 ? `0${month}` : month;
      let formattedDateOfBirth = `${day}/${month}/${year}`;
      //Nếu email, giới tính, địa chỉ null thì không hiển thị
      let email = item.Email;
      email = email === null ? "" : email;
      let gender = item.GenderName;
      gender = gender === null ? "" : gender;
      let address = item.Address;
      address = address === null ? "" : address;

      tr.innerHTML = `<td class="m-data-center"><input type="checkbox" /></td>
                    <td empIdCell style="display: none;">${item.EmployeeId}</td>
                    <td>${stt}</td>
                    <td>${item.EmployeeCode}</td>
                    <td>${item.FullName}</td>
                    <td>${gender}</td>
                    <td>${formattedDateOfBirth}</td>
                    <td>${email}</td>
                    <td>${address}</td>
                    <td>
                      <div class="m-table-tool">
                        <div class="m-edit m-tool-icon"></div>
                        <div class="m-delete m-tool-icon"></div>
                      </div>
                    </td>`;
      table.querySelector("tbody").append(tr);
      stt++;
    }
    document.querySelector(".m-pagination-left b").innerHTML = stt - 1;
  }
  /**
   * Click vào button Thêm mới
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  btnAddOnClick() {
    try {
      //Hiển thị form Thêm mới
      //1. Lấy ra element của form thêm mới
      const dlgDetail = document.querySelector("#dlgDetail");
      //2. Set hiển thị form
      dlgDetail.style.visibility = "visible";
      //Đổi tên tiêu đề dialog
      dlgDetail.querySelector(".m-dialog-title").innerHTML =
        "Thêm mới nhân viên";
      dlgDetail.querySelector("#empId").focus();
    } catch (error) {
      console.error(error);
    }
  }
  /**
   * Thêm mới dữ liệu vào API khi hợp lệ
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  btnSaveOnClick() {
    try {
      if (this.formMode === "add") {
        //1. Thực hiện kiểm tra dữ liệu hợp lệ
        const validateRequired = this.checkRequiredInput();
        const validateDateInput = this.checkDateInput();
        //2. Nếu lỗi đưa ra dialog thông báo
        if (validateRequired.errors.length > 0) {
          this.displayErrorMessage(validateRequired);
        } else if (validateDateInput.errors.length > 0) {
          this.displayErrorMessage(validateDateInput);
        } else {
          //3. Thêm mới nhân viên vào API khi dữ liệu hợp lệ
          this.loadDataAddNew();
        }
      } else if (this.formMode === "edit") {
        //1. Thực hiện kiểm tra dữ liệu hợp lệ
        const validateRequired = this.checkRequiredInput();
        const validateDateInput = this.checkDateInput();
        //2. Nếu lỗi đưa ra dialog thông báo
        if (validateRequired.errors.length > 0) {
          this.displayErrorMessage(validateRequired);
        } else if (validateDateInput.errors.length > 0) {
          this.displayErrorMessage(validateDateInput);
        } else {
          //3. Cập nhật nhân viên vào API khi dữ liệu hợp lệ
          this.loadDataEditEmployee();
        }
      }
    } catch (error) {
      console.error(error);
    }
  }
  /**
   * Thêm một thông báo lỗi vào sau các ô input nếu dữ liệu không hợp lệ
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
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
        if (input.hasAttribute("required")) {
          errorElementMsg.textContent = "Thông tin này không được để trống!";
        } else if (input.hasAttribute("inputDate")) {
          const lable = input.previousElementSibling;
          errorElementMsg.textContent = `${lable.textContent} không được lớn hơn ngày hiện tại!`;
        }
        input.after(errorElementMsg);
      }
    } catch (error) {
      console.log(error);
    }
  }
  /**
   * Kiểm tra các ô input có attr là required
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
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
        } else if (input.classList.contains("m-input-error") || nextElement) {
          input.classList.remove("m-input-error");
          while (input.nextElementSibling) {
            input.nextElementSibling.remove();
          }
        }
        if (
          (value !== null || value !== "" || value !== undefined) &&
          input.hasAttribute("required") &&
          input.hasAttribute("email")
        ) {
          if (this.checkEmail(value) === false) {
            if (!input.classList.contains("m-input-error") || !nextElement) {
              //Thêm border màu đỏ
              input.classList.add("m-input-error");
              let errorElementMsg = document.createElement("div");
              errorElementMsg.classList.add("m-input-error-msg");
              errorElementMsg.textContent = "Email không đúng định dạng!";
              input.after(errorElementMsg);
              result.inputInvalid.push(input);
              result.errors.push("Email không đúng định dạng!");
            }
          } else {
            if (input.classList.contains("m-input-error") || nextElement) {
              input.classList.remove("m-input-error");
              nextElement.remove();
            }
          }
        }
      }
      return result;
    } catch (error) {
      console.log(error);
    }
  }
  /**
   * Kiểm tra ngày nhập vào có lớn hơn ngày hiện tại không
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  checkDateInput() {
    try {
      let result = {
        inputInvalid: [],
        errors: [],
      };
      let inputs = document.querySelectorAll("#dlgDetail input[inputDate]");
      for (const input of inputs) {
        let value = input.value;
        const nextElement = input.nextElementSibling;
        if (value) value = new Date(value);
        if (value > new Date()) {
          const lable = input.previousElementSibling;
          this.addErrorElementToInputInvalid(input);
          result.inputInvalid.push(input);
          result.errors.push(
            `${lable.textContent} không được lớn hơn ngày hiện tại!`
          );
        } else if (input.classList.contains("m-input-error") || nextElement) {
          input.classList.remove("m-input-error");
          nextElement.remove();
        }
      }
      return result;
    } catch (error) {
      console.error(error);
    }
  }
  /**
   * Kiểm tra chuỗi email nhập vào có hợp lệ không
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  checkEmail(email) {
    try {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      return emailRegex.test(email);
    } catch (error) {
      console.error(error);
    }
  }
  /**
   * Hiển thị thông báo lỗi khi dữ liệu không hợp lệ
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  displayErrorMessage(validateInput) {
    try {
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
      for (const msg of validateInput.errors) {
        const m = msg.replace(/\*/g, "");
        let li = document.createElement("li");
        li.textContent = m;
        errorElement.append(li);
      }
      //Focus vào ô input lỗi đầu tiên
      this.inputInvalids = validateInput.inputInvalid;
    } catch (error) {
      console.error(error);
    }
  }
  /**
   * Xóa hết dữ liệu cũ trong dialog
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  clearErrorMessages() {
    try {
      const dlgDetail = document.querySelector("#dlgDetail");
      const errorMessages = dlgDetail.querySelectorAll(".m-input-error-msg");
      const inputErrors = dlgDetail.querySelectorAll("input");
      const radioUncheckeds = dlgDetail.querySelectorAll(".m-radio-icon");
      errorMessages.forEach((errorMessage) => errorMessage.remove());
      for (const inputError of inputErrors) {
        inputError.classList.remove("m-input-error");
        inputError.value = "";
      }
      for (const radio of radioUncheckeds) {
        if (radio.classList.contains("m-radio-icon-checked")) {
          radio.classList.remove("m-radio-icon-checked");
          radio.classList.add("m-radio-icon-unchecked");
        }
      }
    } catch (error) {
      console.log(error);
    }
  }
  /**
   * Hiển thị dialog sửa nhân viên khi click vào nút edit
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
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
  /**
   * Hiển thị dialog xác nhận xóa nhân viên khi click vào nút delete
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  btnDeleteOnClick(employeeId) {
    try {
      let dlgDelete = document.querySelector("#dlgNotice2");
      //Hiện ô dialog
      dlgDelete.style.visibility = "visible";
      //Đổi tên tiêu đề dialog
      dlgDelete.querySelector(".m-dialog-title").innerHTML = "Xác nhận xóa";
      let element = dlgDelete.querySelector(".m-dialog-row");
      element.innerHTML = "";
      //Thêm thông báo lỗi
      element.style.display = "block";
      let div = document.createElement("div");
      this.getEmployee(employeeId).then((employee) => {
        div.textContent = `Bạn có chắc chắn muốn xóa ${employee.EmployeeCode} không?`;
        element.append(div);
      });
      const btnDelete = dlgDelete.querySelector(".m-button");
      btnDelete.addEventListener("click", () => {
        this.deleteEmployee(this.employeeIdForDelete);
        dlgDelete.style.visibility = "hidden";
      });
    } catch (error) {
      console.error(error);
    }
  }
  /**
   * Lấy ra thông tin 1 nhân viên khi xóa
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  async getEmployee(employeeId) {
    const url = `https://cukcuk.manhnv.net/api/v1/Employees/${employeeId}`;

    return fetch(url)
      .then((response) => {
        if (!response.ok) {
          throw new Error("Lỗi lấy thông tin nhân viên: " + employeeId);
        }
        return response.json();
      })
      .then((data) => {
        return data; // Trả về dữ liệu nhân viên từ API
      })
      .catch((error) => {
        console.error("Có lỗi xảy ra khi lấy thông tin nhân viên:", error);
        throw error;
      });
  }
  /**
   * Xóa hết dữ liệu trong bảng khi nhấn nút Refresh
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  btnRefreshOnclick() {
    let dataTable = document.querySelector("#tblEmployee tbody");
    while (dataTable.rows.length > 0) {
      dataTable.deleteRow(0);
    }
  }
  /**
   * Tạo mới 1 đối tượng nhân viên, thêm vào API
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  async loadDataAddNew() {
    try {
      //1. Khai báo các thuộc tính
      const loading = document.querySelector(".m-loading");
      const dlgDetail = document.querySelector("#dlgDetail");
      let employeeCode = dlgDetail.querySelector("#empId").value;
      let employeeName = dlgDetail.querySelector("#empName").value;
      let dateOfBirth = dlgDetail.querySelector("#dateOfBirth").value;
      let gender = null;

      // Lấy tất cả các phần tử có class "m-radio"
      let radioButtons = dlgDetail.querySelectorAll(".m-radio");

      // Lặp qua từng phần tử và thêm sự kiện click
      radioButtons.forEach((radioButton) => {
        // Kiểm tra xem phần tử có class "m-radio-icon-checked" không
        if (
          radioButton
            .querySelector(".m-radio-icon")
            .classList.contains("m-radio-icon-checked")
        ) {
          // Lấy giá trị data-value của phần tử con có class "m-radio-icon"
          let dataValue = radioButton
            .querySelector(".m-radio-icon")
            .getAttribute("data-value");
          gender = dataValue;
        }
      });
      // let position = dlgDetail.querySelector("#position").value;
      // let idcard = dlgDetail.querySelector("#idcard").value;
      // let licensedate = dlgDetail.querySelector("#licensedate").value;
      let department = dlgDetail.querySelector("#department").value;
      // let licensingplace = dlgDetail.querySelector("#licensingplace").value;
      // let address = dlgDetail.querySelector("#address").value;
      // let phonenumber = dlgDetail.querySelector("#phonenumber").value;
      // let landlinenumber = dlgDetail.querySelector("#landlinenumber").value;
      let email = dlgDetail.querySelector("#email").value;
      //let bankaccount = dlgDetail.querySelector("#bankaccount").value;

      //2. Khởi tạo đối tượng mới
      let employee = {
        EmployeeId: "1129f12d-61f3-4960-3f99-04b3a3d51ef2",
        EmployeeCode: employeeCode,
        FullName: employeeName,
        DateOfBirth: dateOfBirth,
        Gender: gender,
        PositionId: "548dce5f-5f29-4617-725d-e2ec561b0f41",
        PersonalTaxCode: "3736202931",
        IdentityDate: "1972-02-22T00:00:00",
        IdentityPlace: "Quảng Bình",
        DepartmentId: department,
        Address: "58 Lâm Thị Hố",
        IdentityNumber: "0209153579",
        Email: email,
        PhoneNumber: "0907710684",
        BankAccount: "0907710684",
        BankName: "MB Bank",
        BankBranch: "Bắc Ninh",
      };
      //3. Hiển thị loading
      loading.style.visibility = "visible";
      //4. Xóa hết dữ liệu trong bảng
      this.btnRefreshOnclick();
      //5. Thêm nhân viên mới vào API
      await this.addEmployeeToAPI(employee);
      //6. Ẩn dialog vào loading
      dlgDetail.style.visibility = "hidden";
      loading.style.visibility = "hidden";
      //7. Load lại data lên bảng
      this.fetchData();
    } catch (error) {
      console.error(error);
    }
  }
  /**
   * Gọi API để thêm mới 1 nhân viên
   * Author: Nguyễn Khắc Cảnh 11/07/2024
   */
  async addEmployeeToAPI(employee) {
    try {
      const response = await fetch(
        "https://cukcuk.manhnv.net/api/v1/Employees",
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(employee),
        }
      );
      if (!response.ok) {
        throw new Error("Thêm nhân viên không thành công!" + response.status);
      }
      //Hiển thị popup thông báo thành công
      const popupSuccess = document.querySelector(
        ".m-popup-item.m-popup-success"
      );
      this.showPopup(popupSuccess, "Thêm mới nhân viên thành công!", "success");
      const data = await response.json();
      return data;
    } catch (error) {
      //Hiển thị popup thông báo thất bại
      const popupError = document.querySelector(".m-popup-item.m-popup-error");
      this.showPopup(popupError, "Thêm mới nhân viên thất bại!", "error");
      console.error(error);
    }
  }
  /**
   * Hiển thị popup
   * Author: Nguyễn Khắc Cảnh 11/07/2024
   */
  showPopup(popupElement, message, type) {
    const popupText = popupElement.querySelector(".m-popup-text-" + type);
    const closePopup = popupElement.querySelector(".m-popup-close");
    if (type === "success") {
      popupText.innerHTML = `<span>Thành công! </span> ${message}`;
    } else if (type === "error") {
      popupText.innerHTML = `<span>Lỗi! </span> ${message}`;
    } else if (type === "warning") {
      popupText.innerHTML = `<span>Cảnh báo! </span> ${message}`;
    } else {
      popupText.innerHTML = `<span>Thông tin! </span> ${message}`;
    }
    popupElement.style.visibility = "visible";
    popupElement.style.top = "10px";
    closePopup.addEventListener("click", () => {
      popupElement.style.visibility = "hidden";
    });
    setTimeout(() => {
      popupElement.style.visibility = "hidden";
    }, 5000);
  }
  /**
   * Gọi API theo mã, binding lên các ô input
   * Author: Nguyễn Khắc Cảnh 11/07/2024
   */
  getDataToEditEmployee(employeeId) {
    try {
      // Gọi API để lấy thông tin nhân viên theo ID
      fetch(`https://cukcuk.manhnv.net/api/v1/Employees/${employeeId}`)
        .then((res) => {
          if (!res.ok) {
            throw new Error("Không tìm thấy thông tin chi tiết về nhân viên!");
          }
          return res.json();
        })
        .then((employee) => {
          const dlgDetail = document.querySelector("#dlgDetail");
          dlgDetail.querySelector("#empId").value = employee.EmployeeCode;
          dlgDetail.querySelector("#empName").value = employee.FullName;

          dlgDetail.querySelector("#dateOfBirth").value =
            this.formatDateForEdit(employee.DateOfBirth);

          let gender = employee.Gender;
          if (gender !== null) {
            let radioIcons = dlgDetail.querySelectorAll(".m-radio-icon");
            // Duyệt qua từng phần tử và thay đổi trạng thái
            radioIcons.forEach((icon) => {
              // Lấy giá trị data-value từ thuộc tính data-value của phần tử
              let dataValue = icon.getAttribute("data-value");
              // Kiểm tra nếu data-value của phần tử này khớp với giá trị của biến gender
              if (dataValue === gender.toString()) {
                // Xóa class 'm-radio-icon-unchecked' và thêm class 'm-radio-icon-checked'
                icon.classList.remove("m-radio-icon-unchecked");
                icon.classList.add("m-radio-icon-checked");
              } else {
                // Nếu không khớp, đảm bảo rằng phần tử có class 'm-radio-icon' chỉ có class 'm-radio-icon-unchecked'
                icon.classList.remove("m-radio-icon-checked");
                icon.classList.add("m-radio-icon-unchecked");
              }
            });
          }

          dlgDetail.querySelector("#position").value = employee.PositionId;
          dlgDetail.querySelector("#idcard").value = employee.IdentityNumber;
          dlgDetail.querySelector("#licensedate").value =
            this.formatDateForEdit(employee.IdentityDate);

          dlgDetail.querySelector("#department").value = employee.DepartmentId;
          dlgDetail.querySelector("#licensingplace").value =
            employee.IdentityPlace;
          dlgDetail.querySelector("#address").value = employee.Address;
          dlgDetail.querySelector("#phonenumber").value = employee.PhoneNumber;
          dlgDetail.querySelector("#landlinenumber").value =
            employee.PhoneNumber;
          dlgDetail.querySelector("#email").value = employee.Email;
          dlgDetail.querySelector("#bankaccount").value = employee.PhoneNumber;
          dlgDetail.querySelector("#bankname").value = "MB Bank";
          dlgDetail.querySelector("#branch").value = "Bắc Ninh";
          //dlgDetail.style.visibility = "visible";
        });
    } catch (error) {
      const popupError = document.querySelector(".m-popup-item.m-popup-error");
      this.showPopup(popupError, "Sửa thông tin nhân viên thất bại!", "error");
      console.error(error);
    }
  }
  /**
   * Sửa thông tin nhân viên, dữ liệu lấy từ các ô input nhập vào
   * Author: Nguyễn Khắc Cảnh 11/07/2024
   */
  async loadDataEditEmployee() {
    try {
      //1. Khai báo các thuộc tính
      const loading = document.querySelector(".m-loading");
      const dlgDetail = document.querySelector("#dlgDetail");
      let employeeId = this.employeeIdForEdit;
      let employeeCode = dlgDetail.querySelector("#empId").value;
      let employeeName = dlgDetail.querySelector("#empName").value;
      let dateOfBirth = dlgDetail.querySelector("#dateOfBirth").value;
      let gender = null;
      // Lấy tất cả các phần tử có class "m-radio"
      let radioButtons = dlgDetail.querySelectorAll(".m-radio");

      // Lặp qua từng phần tử và thêm sự kiện click
      radioButtons.forEach((radioButton) => {
        // Kiểm tra xem phần tử có class "m-radio-icon-checked" không
        if (
          radioButton
            .querySelector(".m-radio-icon")
            .classList.contains("m-radio-icon-checked")
        ) {
          // Lấy giá trị data-value của phần tử con có class "m-radio-icon"
          let dataValue = radioButton
            .querySelector(".m-radio-icon")
            .getAttribute("data-value");
          gender = dataValue;
        }
      });
      // let position = dlgDetail.querySelector("#position").value;
      let idcard = dlgDetail.querySelector("#idcard").value;
      let licensedate = dlgDetail.querySelector("#licensedate").value;
      let department = dlgDetail.querySelector("#department").value;
      let licensingplace = dlgDetail.querySelector("#licensingplace").value;
      let address = dlgDetail.querySelector("#address").value;
      let phonenumber = dlgDetail.querySelector("#phonenumber").value;
      //let landlinenumber = dlgDetail.querySelector("#landlinenumber").value;
      let email = dlgDetail.querySelector("#email").value;
      //let bankaccount = dlgDetail.querySelector("#bankaccount").value;

      //2. Khởi tạo đối tượng mới
      let updateEmployee = {
        EmployeeId: employeeId,
        EmployeeCode: employeeCode,
        FullName: employeeName,
        DateOfBirth: dateOfBirth,
        Gender: gender,
        PositionId: "548dce5f-5f29-4617-725d-e2ec561b0f41",
        PersonalTaxCode: idcard,
        IdentityDate: licensedate,
        IdentityPlace: licensingplace,
        DepartmentId: department,
        Address: address,
        IdentityNumber: idcard,
        Email: email,
        PhoneNumber: phonenumber,
        BankAccount: phonenumber,
        BankName: "MB Bank",
        BankBranch: "Bắc Ninh",
      };
      //3. Hiển thị loading
      loading.style.visibility = "visible";
      //4. Xóa hết dữ liệu trong bảng
      this.btnRefreshOnclick();
      //5. Thêm nhân viên mới vào API
      await this.editEmployeeToAPI(updateEmployee);
      //6. Ẩn dialog vào loading
      dlgDetail.style.visibility = "hidden";
      loading.style.visibility = "hidden";
      //7. Load lại data lên bảng
      this.fetchData();
    } catch (error) {
      console.error(error);
    }
  }
  /**
   * Gọi API để thêm mới 1 nhân viên
   * Author: Nguyễn Khắc Cảnh 11/07/2024
   */
  async editEmployeeToAPI(updateEmployee) {
    try {
      const response = await fetch(
        "https://cukcuk.manhnv.net/api/v1/Employees",
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify(updateEmployee),
        }
      );
      if (!response.ok) {
        throw new Error(
          "Sửa thông tin nhân viên không thành công!" + response.status
        );
      }
      //Hiển thị popup thông báo thành công
      const popupSuccess = document.querySelector(
        ".m-popup-item.m-popup-success"
      );
      this.showPopup(
        popupSuccess,
        "Sửa thông tin nhân viên thành công!",
        "success"
      );
      const data = await response.json();
      return data;
    } catch (error) {
      //Hiển thị popup thông báo thất bại
      const popupError = document.querySelector(".m-popup-item.m-popup-error");
      this.showPopup(popupError, "Sửa thông tin nhân viên thất bại!", "error");
      console.error(error);
    }
  }
  /**
   * Gọi API để xóa 1 nhân viên
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  async deleteEmployee(employeeId) {
    const url = `https://cukcuk.manhnv.net/api/v1/Employees/${employeeId}`;
    const options = {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
      },
    };

    return fetch(url, options)
      .then((response) => {
        if (!response.ok) {
          throw new Error("Không xóa được bản ghi này!");
        }
        // Kiểm tra nếu có dữ liệu trả về từ server
        const contentType = response.headers.get("content-type");
        if (contentType && contentType.includes("application/json")) {
          return response.json(); // Parse JSON nếu có
        } else {
          return {}; // Hoặc trả về một đối tượng rỗng nếu không có dữ liệu JSON
        }
        //return response.json();
      })
      .then((data) => {
        //Hiển thị popup thông báo thành công
        const popupSuccess = document.querySelector(
          ".m-popup-item.m-popup-success"
        );
        this.showPopup(popupSuccess, "Xóa nhân viên thành công!", "success");
        this.btnRefreshOnclick();
        this.fetchData();
        return data; // Trả về dữ liệu phản hồi từ API nếu cần
      })
      .catch((error) => {
        //Hiển thị popup thông báo thất bại
        const popupError = document.querySelector(
          ".m-popup-item.m-popup-error"
        );
        this.showPopup(popupError, "Xóa nhân viên thất bại!", "error");
        console.error(error);
        throw error; // Ném lỗi để bên gọi hàm có thể xử lý lỗi này nếu cần
      });
  }
  /**
   * Định dạng ngày để hiển thị lên input type = date
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  formatDateForEdit(DateOfBirth) {
    let dateObj = new Date(DateOfBirth);
    // Lấy ngày tháng (đã chuyển đổi đúng múi giờ)
    let year = dateObj.getFullYear();
    let month = ("0" + (dateObj.getMonth() + 1)).slice(-2); // Tháng bắt đầu từ 0 nên cần +1 và định dạng lại về đúng format MM
    let day = ("0" + dateObj.getDate()).slice(-2); // Định dạng ngày về đúng format DD

    // Format ngày tháng thành YYYY-MM-DD (định dạng mà input type=date yêu cầu)
    let formattedDate = `${year}-${month}-${day}`;
    return formattedDate;
  }
  /**
   * Gọi API, hiển thị danh sách phòng ban lên select, option
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  fetchDepartmentsAndPopulateSelect() {
    const selectElement = document.getElementById("department");

    // Gọi API
    const apiUrl = "https://cukcuk.manhnv.net/api/v1/Departments";

    fetch(apiUrl)
      .then((response) => response.json())
      .then((data) => {
        selectElement.innerHTML =
          '<option value="">--Chọn phòng ban--</option>';

        // Nối các tùy chọn dựa trên phản hồi API
        data.forEach((department) => {
          const option = document.createElement("option");
          option.value = department.DepartmentId;
          option.textContent = department.DepartmentName;
          selectElement.appendChild(option);
        });
      })
      .catch((error) => {
        console.error(error);
      });
  }
  /**
   * Hàm xử lý phân trang
   * Author: Nguyễn Khắc Cảnh 12/07/2024
   */
  paginationForData() {
    const pagination = document.querySelector(".m-pagination");
    const recordsPerPageSelect = pagination.querySelector("#recordsPerPage");
    const prevBtn = pagination.querySelector(".m-pagination-prev");
    const nextBtn = pagination.querySelector(".m-pagination-next");
    let recordsPerPage = parseInt(recordsPerPageSelect.value, 10);
    // Xử lý sự kiện khi thay đổi số lượng bản ghi trên mỗi trang
    recordsPerPageSelect.addEventListener("change", () => {
      recordsPerPage = parseInt(recordsPerPageSelect.value, 10);
      this.currentPage = 1; // Trở về trang đầu tiên khi thay đổi số lượng bản ghi
      this.btnRefreshOnclick();
      this.loadData(this.employeesData);
    });

    // Xử lý sự kiện khi click vào nút Previous
    prevBtn.addEventListener("click", () => {
      if (this.currentPage > 1) {
        this.currentPage--;
        this.btnRefreshOnclick();
        this.loadData(this.employeesData);
      }
    });

    // Xử lý sự kiện khi click vào nút Next
    nextBtn.addEventListener("click", () => {
      if (
        this.currentPage < Math.ceil(this.employeesData.length / recordsPerPage)
      ) {
        this.currentPage++;
        this.btnRefreshOnclick();
        this.loadData(this.employeesData);
      }
    });
  }

  // async fetchDataToExport() {
  //   try {
  //     const response = await fetch(
  //       "https://cukcuk.manhnv.net/api/v1/Employees"
  //     );
  //     if (!response.ok) {
  //       throw new Error("Phản hồi của mạng không ổn!");
  //     }
  //     const data = await response.json();
  //     this.employeesData = data; // Lưu dữ liệu từ API vào employeesData
  //   } catch (error) {
  //     console.error("Lỗi tìm nạp dữ liệu:", error);
  //   }
  // }

  exportDataToExcel() {
    if (this.employeesData.length === 0) {
      console.error("Không có dữ liệu để xuất");
      return;
    }

    const headers = [
      "Mã nhân viên",
      "Họ tên",
      "Ngày sinh",
      "Giới tính",
      "Vị trí",
      "Số CMTND",
      "Ngày cấp",
      "Phòng ban",
      "Nơi cấp",
      "Địa chỉ",
      "ĐT di động",
      "ĐT cố định",
      "Email",
      "Tài khoản ngân hàng",
      "Tên ngân hàng",
      "Chi nhánh",
    ]; // Định nghĩa các cột

    const rows = this.employeesData.map((employee) => [
      employee.EmployeeCode,
      employee.FullName,
      employee.DateOfBirth,
      employee.GenderName,
      employee.PositionName,
      employee.PersonalTaxCode,
      employee.IdentityDate,
      employee.DepartmentName,
      employee.IdentityPlace,
      employee.Address,
      employee.PhoneNumber,
      employee.PhoneNumber,
      employee.Email,
      employee.PhoneNumber,
      "MB Bank",
      "Bắc Ninh",
    ]);

    const sheetData = [headers, ...rows]; // Dữ liệu của sheet

    const sheetName = "Danh sách nhân viên"; // Tên của sheet trong Excel

    const wb = XLSX.utils.book_new();
    const ws = XLSX.utils.aoa_to_sheet(sheetData);
    XLSX.utils.book_append_sheet(wb, ws, sheetName);

    XLSX.writeFile(wb, "employee_data.xlsx"); // Tạo và tải file Excel về
  }
}
