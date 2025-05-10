<template>
    <CRow>
        <CCol :xs="12">
            <CCard class="mb-4">
                <CCardHeader>
                    <strong>User Profile</strong>
                </CCardHeader>
                <CCardBody>
                    <CForm @submit.prevent="savePage">
                        <CRow class="mb-3">
                            <CCol sm="12">
                                <CAvatar :src="avatar" @click="askToChange" size="xl" color="success">{{ avatarUserName }}</CAvatar>
                            </CCol>
                        </CRow>
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                User Name
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.userName" type="text" id="userName" name="userName"
                                    readonly plain-text />
                            </CCol>
                        </CRow>
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                Full Name
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.fullName" type="text" id="fullName" name="fullName" />
                            </CCol>
                        </CRow>
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                E-Mail
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.email" type="email" id="email" name="email" readonly
                                    plain-text />
                            </CCol>
                        </CRow>
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                Age
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.age" type="number" id="age" name="age" />
                            </CCol>
                        </CRow>
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                Address
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.address" type="text" id="address" name="address" />
                            </CCol>
                        </CRow>
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                Roles
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.roles" readonly plain-text />
                            </CCol>
                        </CRow>
                        <CAlert :color="mainMessage.color" :visible="mainMessage.shown" dismissible
                            @close="() => { mainMessage.shown = false }">{{ mainMessage.message }}</CAlert>

                        <CButton color="primary" type="submit">Save</CButton>
                        &nbsp;
                        <CButton color="warning" @click="() => { changePassword = true }">Change Password</CButton>
                        &nbsp;
                        <CButton color="warning" @click="cancel">Close</CButton>
                    </CForm>
                </CCardBody>
            </CCard>
        </CCol>
    </CRow>
    <CModal :visible="changePassword" @close="() => { changePassword = false }" size="lg">
        <CModalHeader>
            <CModalTitle>Change Password</CModalTitle>
        </CModalHeader>
        <CModalBody>
            <CRow class="mb-3">
                <CFormLabel for="name" class="col-sm-2 col-form-label">
                    Old Password
                </CFormLabel>
                <CCol sm="10">
                    <CFormInput v-model="oldPassword" type="password" id="password" name="password" />
                </CCol>
            </CRow>
            <CRow class="mb-3">
                <CFormLabel for="name" class="col-sm-2 col-form-label">
                    New Password
                </CFormLabel>
                <CCol sm="10">
                    <CFormInput v-model="newPassword" type="password" id="password" name="password" />
                </CCol>
            </CRow>
            <CAlert :color="userPasswordMessage.color" :visible="userPasswordMessage.shown" dismissible
                @close="() => { userPasswordMessage.shown = false }">{{ userPasswordMessage.message }}</CAlert>
        </CModalBody>
        <CModalFooter>
            <CButton color="secondary" @click="changePasswordAction()">Yes</CButton>
            <CButton color="primary" @click="() => { changePassword = false }">
                Close
            </CButton>
        </CModalFooter>
    </CModal>
    <CModal :visible="changeImageShown" @close="() => { changeImageShown = false }">
        <CModalBody>
            <CRow class="mb-3">
                <CCol sm="10">
                    <CFormInput type="file" @change="handleFileUpload" label="Upload Image"/>
                </CCol>
            </CRow>
            <CAlert :color="mainMessage.color" :visible="mainMessage.shown" dismissible
                    @close="() => { mainMessage.shown = false }">{{ mainMessage.message }}</CAlert>
        </CModalBody>
        <CModalFooter>
            <CButton color="secondary" @click="saveUserImage">Submit</CButton>
            <CButton color="primary" @click="() => { changeImageShown = false }">
                Close
            </CButton>
        </CModalFooter>
    </CModal>
</template>
<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
//#region main variable
const avatar = ref('')
const avatarUserName = ref('')

const chatIdPlaceholder = ref('Telegram Not Registered')

const pageData = ref({
    id: '',
    userName: '',
    fullName: '',
    email: '',
    age: 0,
    address: '',
    dataStatus: 1,
    roles: ['User'],
    userTelegramId: 0,
    userTelegram: {
        chatId: '',
    }
})

const mainMessage = ref({
    message: '',
    shown: false,
    color: 'danger'
})
//#endregion main variable
//#region change password variable
const changePassword = ref(false)
const oldPassword = ref('')
const newPassword = ref('')

const userPasswordMessage = ref({
    message: '',
    shown: false,
    color: 'danger'
})
//#endregion change password variable
//#region user image variable
const changeImageShown = ref(false)
var userImageData = {}
//#endregion user image variable

//#region Main
const getAccessToken = () => {
    // const storage = localStorage.getItem("user")
    const storage = sessionStorage.getItem('user')
    if (storage) {
        let user = JSON.parse(storage)

        return user.token
    }

    return null
}

const fetchPageData = (code) => {
    fetch(`api/users?id=${code}`, {
        method: "get",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            Authorization: `Bearer ${getAccessToken()}`,
        }
    })
        .then(result => {
            if (result.ok) {
                result.json().then(data => {
                    console.log("data get ==> ", data)
                    pageData.value.id = data.id
                    pageData.value.userName = data.userName
                    pageData.value.fullName = data.fullName
                    pageData.value.email = data.email
                    pageData.value.age = data.age
                    pageData.value.address = data.address
                    pageData.value.dataStatus = data.dataStatus
                    pageData.value.roles = data.roles

                    if (data.userTelegramId) {
                        pageData.value.userTelegramId = data.userTelegramId
                        pageData.value.userTelegram.chatId = data.userTelegram.chatId

                        if(data.userTelegram.tokenRegistration)
                            chatIdPlaceholder.value = 'Telegram Not Registered, your registration token is ' + data.userTelegram.tokenRegistration
                    }

                    avatarUserName.value = generateAvatarStringFromName(data.fullName)

                    getUserImage(code)
                })
            } else {
                result.json().then(data => {
                    console.log("error get ==> ", data)
                    mainMessage.value.shown = true
                    mainMessage.value.message = data.message
                    mainMessage.value.color = 'danger'
                })
            }
        })
}

const generateAvatarStringFromName = (fullName) => {
    const nameParts = fullName.split(' ');

    let initials = nameParts[0].slice(0, 2)
    if (nameParts.length > 1) {
        initials = nameParts[0].charAt(0) + nameParts[nameParts.length - 1].charAt(0)
    }

    return initials;
}

onMounted(() => {

    const storage = sessionStorage.getItem('user')

    if (storage) {
        let user = JSON.parse(storage)

        fetchPageData(user.id)
    }
})

const savePage = () => {

    // Update existing page using pageData
    updatePage().then(result => {
        if (result.ok) {
            mainMessage.value.shown = true
            mainMessage.value.message = 'Data updated successfully'
            mainMessage.value.color = 'success'
        } else {
            result.json().then(data => {
                mainMessage.value.shown = true
                mainMessage.value.message = data.message
                mainMessage.value.color = 'danger'
            })
        }
    })

    //router.push('/dashboard')
}

const updatePage = () => {
    return fetch(`api/users`, {
        method: "put",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            Authorization: `Bearer ${getAccessToken()}`,
        },
        body: JSON.stringify(pageData.value)
    })
}

const cancel = () => {
    router.replace('/dashboard') // Redirect to the list of pages without saving
}

//#endregion Main
//#region Change Password
const changePasswordAction = () => {
    fetch("api/changePassword", {
        method: "post",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            Authorization: `Bearer ${getAccessToken()}`,
        },
        body: JSON.stringify({
            userId: pageData.value.id,
            oldPassword: oldPassword.value,
            newPassword: newPassword.value
        })
    })
        .then(result => {
            if (result.ok) {
                result.json().then(data => {
                    userPasswordMessage.value.shown = true
                    userPasswordMessage.value.message = "Password has been change"
                    userPasswordMessage.value.color = 'success'
                })
            } else {
                result.json().then(data => {
                    console.log("err add ==>", data)
                    userPasswordMessage.value.shown = true
                    userPasswordMessage.value.message = "Failed to change Password, " + data.message
                    userPasswordMessage.value.color = 'danger'
                })
            }
        })
}
//#endregion Change Password
//#region user image
const getUserImage = (id) => {
    fetch(`api/users/image?id=${id}`, {
        method: "get",
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json',
            Authorization: `Bearer ${getAccessToken()}`,
        }
    })
        .then(result => {
            if (result.ok) {
                result.blob().then(blob => {
                    if (blob && blob.size > 0)
                        avatar.value = window.URL.createObjectURL(blob);
                })
            } else {
                result.json().then(data => {
                    console.log("error get ==> ", data)
                    mainMessage.value.shown = true
                    mainMessage.value.message = data.message
                    mainMessage.value.color = 'danger'
                })
            }
        })
}

const askToChange = () => {
    changeImageShown.value = true
}

const handleFileUpload = (event) => {

    if (event.target.files[0]['size'] > 524288) {
        mainMessage.value.shown = true
        mainMessage.value.message = 'Invalid image size (max 512 KB)'
        mainMessage.value.color = 'danger'
        return
    }

    if (['image/png', 'image/jpeg', 'image/svg'].includes(event.target.files[0]['type'])) {
        userImageData = event.target.files[0];
    } else {
        mainMessage.value.shown = true
        mainMessage.value.message = 'Invalid image type (png, jpg or svg only)'
        mainMessage.value.color = 'danger'
    }
    
}

const saveUserImage = () => {
    const formdata = new FormData();
    formdata.append("formFile", userImageData);

    const requestOptions = {
      method: "POST",
      headers: {
            mimeType: "multipart/form-data",
            contentType: 'false',
            Authorization: `Bearer ${getAccessToken()}`,
        },
      body: formdata
    };

    fetch(`api/users/image?id=${pageData.value.id}`, requestOptions)
      .then((result) => {
          if (result.ok) {
            changeImageShown.value = false
            avatar.value = window.URL.createObjectURL(userImageData)
          } else {
              result.json().then(data => {
                mainMessage.value.shown = true
                mainMessage.value.message = data.message
                mainMessage.value.color = 'danger'
            })
          }
      })
      .catch((error) => {
          mainMessage.value.shown = true
          mainMessage.value.message = error
          mainMessage.value.color = 'danger'
    });
}
//#endregion user image
</script>
