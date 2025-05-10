<template>
    <CRow>
        <CCol :xs="12">
            <CCard class="mb-4">
                <CCardHeader>
                    <strong>{{ mode === 'add' ? 'Add' : 'Edit' }} User</strong>
                </CCardHeader>
                <CCardBody>
                    <CRow class="mb-3">
                        <CCol sm="12">
                            <CButton color="warning" @click="() => { resetPassword = true }">Reset Password</CButton>
                        </CCol>
                    </CRow>
                    <hr />
                    <CRow class="mb-3">
                        <CCol sm="12">
                            <CAvatar :src="avatar" @click="askToChange" size="xl" color="success">{{ avatarUserName }}</CAvatar>
                        </CCol>
                    </CRow>
                    <CForm @submit.prevent="savePage">
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                User Name
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.userName"
                                            type="text"
                                            id="userName"
                                            name="userName"
                                            :readonly="mode == 'edit'"
                                            :plain-text="mode == 'edit'" />
                            </CCol>
                        </CRow>
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                Full Name
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.fullName"
                                            type="text"
                                            id="fullName"
                                            name="fullName" />
                            </CCol>
                        </CRow>
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                E-Mail
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.email"
                                            type="email"
                                            id="email"
                                            name="email" />
                            </CCol>
                        </CRow>
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                Age
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.age"
                                            type="number"
                                            id="age"
                                            name="age"
                                            :readonly="mode == 'edit'"
                                            :plain-text="mode == 'edit'" />
                            </CCol>
                        </CRow>
                        <CRow class="mb-3">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                Address
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormInput v-model="pageData.address"
                                            type="text"
                                            id="address"
                                            name="address" />
                            </CCol>
                        </CRow>
                        <CRow class="mb-3" :hidden="pageData.userName == userLogin.name">
                            <CFormLabel for="name" class="col-sm-2 col-form-label">
                                Roles
                            </CFormLabel>
                            <CCol sm="10">
                                <CFormCheck id="admin" value="Admin" label="Admin" v-model="pageData.roles" />
                                <CFormCheck id="user" value="User" label="User" v-model="pageData.roles" />
                            </CCol>
                        </CRow>
                        <CAlert :color="mainMessage.color" :visible="mainMessage.shown" dismissible
                                @close="() => { mainMessage.shown = false }">{{ mainMessage.message }}</CAlert>

                        <CButton color="primary" type="submit">
                            {{
              mode === 'add' ? 'Add' : 'Save'
                            }}
                        </CButton>
                        &nbsp;
                        <CButton color="warning" @click="cancel">Cancel</CButton>
                    </CForm>
                </CCardBody>
            </CCard>
        </CCol>
    </CRow>
    <CModal :visible="resetPassword"
            @close=" () => { resetPassword = false }"
            size="lg">
        <CModalHeader>
            <CModalTitle>Reset Password</CModalTitle>
        </CModalHeader>
        <CModalBody>
            <CRow class="mb-3">
                <CFormLabel for="name" class="col-sm-2 col-form-label">
                    New Password
                </CFormLabel>
                <CCol sm="10">
                    <CFormInput v-model="newPassword"
                                type="password"
                                id="password"
                                name="password" />
                </CCol>
            </CRow>
            <CAlert :color="mainMessage.color" :visible="mainMessage.shown" dismissible
                    @close="() => { mainMessage.shown = false }">{{ mainMessage.message }}</CAlert>
        </CModalBody>
        <CModalFooter>
            <CButton color="secondary" @click="resetPasswordAction()">Yes</CButton>
            <CButton color="primary"
                     @click="() => { resetPassword = false }">
                Cancel
            </CButton>
        </CModalFooter>
    </CModal>
    <CModal :visible="changeImageShown" @close="() => { changeImageShown = false }">
        <CModalBody>
            <CRow class="mb-3">
                <CCol sm="10">
                    <CFormInput type="file" @change="handleFileUpload" label="Upload Image" />
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
    import { useRouter, useRoute } from 'vue-router'

    const userLogin = ref({
        name: ''
    })
    const router = useRouter()
    const route = useRoute()
    const mode = ref('add') // Set this to 'edit' in the edit mode route

    const pageData = ref({
        id: '',
        userName: '',
        fullName: '',
        email: '',
        age: 0,
        address: '',
        dataStatus: 1,
        roles: ['User'],
    })

    const resetPassword = ref(false)
    const newPassword = ref('')

    const mainMessage = ref({
        message: '',
        shown: false,
        color: 'danger'
    })

    const avatar = ref('')
    const avatarUserName = ref('')
    const changeImageShown = ref(false)
    var userImageData = null

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

                        avatarUserName.value = generateAvatarStringFromName(data.fullName);

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
            userLogin.value = JSON.parse(storage)
        }
        // Check if the component is in edit mode based on the route parameter
        const pageId = route.params.id // Adjust the route parameter name if needed
        if (pageId) {
            mode.value = 'edit'
            // Fetch page data using the pageId and update pageData
            fetchPageData(pageId)
        }
    })

    const savePage = () => {
        if (mode.value === 'add') {
            // Save new page using pageData
            saveNewPage().then(result => {
                if (result.ok) {
                    console.log('addd ==> ', result)
                    router.replace('/sys/users').then(() => {
                        window.location.reload()
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
        } else {
            // Update existing page using pageData
            updatePage().then(result => {
                if (result.ok) {
                    console.log('edit ==> ', result)
                    router.replace('/sys/users').then(() => {
                        window.location.reload()
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
    }

    const saveNewPage = () => {
        return fetch("api/users", {
            method: "post",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                Authorization: `Bearer ${getAccessToken()}`,
            },
            body: JSON.stringify(pageData.value)
        })
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

    const resetPasswordAction = () => {
        fetch("api/resetPassword", {
            method: "post",
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                Authorization: `Bearer ${getAccessToken()}`,
            },
            body: JSON.stringify({
                userId: pageData.value.id,
                password: newPassword.value
            })
        })
            .then(result => {
                if (result.ok) {
                    result.json().then(data => {
                        resetPassword.value = false
                        mainMessage.value.shown = true
                        mainMessage.value.message = "Password has been reset"
                        mainMessage.value.color = 'success'
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

    const cancel = () => {
      //router.push('/sys/users') // Redirect to the list of pages without saving
        router.go(-1)
    }

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
        if (userImageData == null) {
            mainMessage.value.shown = true
            mainMessage.value.message = 'No image file registered'
            mainMessage.value.color = 'danger'
        }

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
