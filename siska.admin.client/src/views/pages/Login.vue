<template>
  <div class="wrapper min-vh-100 d-flex flex-row align-items-center">
    <CContainer>
      <CRow class="justify-content-center">
        <CCol :md="8">
          <CCardGroup>
            <CCard class="p-4">
              <CCardBody>
                <CForm @submit.prevent="login">
                  <h1>Login</h1>
                  <p class="text-body-secondary">Sign In to your account</p>
                  <CInputGroup class="mb-3">
                    <CInputGroupText>
                      <CIcon icon="cil-user" />
                    </CInputGroupText>
                    <CFormInput placeholder="Username"
                                autocomplete="username"
                                v-model="username" />
                  </CInputGroup>
                  <CInputGroup class="mb-4">
                    <CInputGroupText>
                      <CIcon icon="cil-lock-locked" />
                    </CInputGroupText>
                    <CFormInput type="password"
                                placeholder="Password"
                                autocomplete="current-password"
                                v-model="password" />
                  </CInputGroup>
                  <CRow>
                    <CCol :xs="12">
                      <CAlert color="danger" :visible="errorMessage" dismissible @close="() => { errorMessage = false }">{{ messageError }}</CAlert>
                      <CButton type="submit" color="primary" class="px-4"> Login </CButton>
                    </CCol>
                  </CRow>
                </CForm>
              </CCardBody>
            </CCard>
          </CCardGroup>
        </CCol>
      </CRow>
    </CContainer>
  </div>
</template>
<script setup>
    import { ref } from 'vue'
    import { useRouter } from 'vue-router'

    const username = ref('')
    const password = ref('')

    const errorMessage = ref(false)
    const messageError = ref('')

    const router = useRouter()

    sessionStorage.removeItem('user')

    const login = () => {
        try {
            fetch("api/signin", {
                method: "post",
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                //make sure to serialize your JSON body
                body: JSON.stringify({
                    login: username.value,
                    password: password.value,
                })
            })
            .then(result => {
                if (result.ok) {
                    result.json().then(data => {
                        sessionStorage.setItem('user', JSON.stringify(data))
                        router.push({ name: 'Home' })
                    })
                } else {
                    result.json().then(data => {
                        errorMessage.value = true
                        messageError.value = data.message
                    })
                }
            })
        } catch (error) {
            alert('Error during login:', error)
        }
    }
</script>
