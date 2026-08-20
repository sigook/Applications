<template>
  <div>
    <b-loading v-model="isLoading"></b-loading>
    <form class="form-md" @submit.prevent="validateForm">
      <b-steps v-model="activeStep" animated mobile-mode="compact" :has-navigation="false">
        <b-step-item step="1" label="Basic" :clickable="false">
          <h1 class="title has-text-centered">Basic Information</h1>
          <div class="columns is-multiline">
            <div class="column is-12">
              <div class=" container-image mx-auto my-2">
                <UploadImage @imageSelected="(profileImg) => saveImage(profileImg)" :edited-image="worker.profileImage"
                  @onUpload="() => pubSub.subscribe('file')" @finishUpload="() => pubSub.unsubscribe()"
                  class="upload-image-spacing" />
                <p class="fz-2">
                  <i>Please upload a photo taken in front of a plain white or off-white background</i>
                </p>
              </div>
            </div>
            <div class="column is-6 is-3-desktop">
              <b-field :type="errors.firstName ? 'is-danger' : ''"
                :message="errors.firstName || ''">
                <template #label>
                  {{ 'Name' }} <span class="has-text-danger">*</span>
                </template>
                <b-input type="text" v-model="firstName" name="name" />
              </b-field>
            </div>
            <div class="column is-6 is-3-desktop">
              <b-field :type="errors.lastName ? 'is-danger' : ''"
                :message="errors.lastName || ''">
                <template #label>
                  {{ 'Last Name' }} <span class="has-text-danger">*</span>
                </template>
                <b-input type="text" v-model="lastName" name="lastname" />
              </b-field>
            </div>
            <div class="column is-6 is-3-desktop">
              <b-field :type="errors.birthDay ? 'is-danger' : ''"
                :message="errors.birthDay || ''">
                <template #label>
                  {{ 'Date of birth' }} <span class="has-text-danger">*</span>
                </template>
                <b-datepicker v-model="birthDay" name="birthday" :focused-date="disabledDates"
                  :max-date="disabledDates">
                </b-datepicker>
              </b-field>
            </div>
            <div class="column is-6 is-3-desktop">
              <b-field :type="errors.gender ? 'is-danger' : ''"
                :message="errors.gender || ''">
                <template #label>
                  {{ 'Gender' }} <span class="has-text-danger">*</span>
                </template>
                <b-select v-model="gender" name="gender" expanded>
                  <option v-for="item in genders" v-bind:key="item.id" :value="item.id">
                    {{ item.value }}
                  </option>
                </b-select>
              </b-field>
            </div>
          </div>
          <AddressComponent ref="addressComponent" v-model:model="worker.location" @isLoading="(value) => isLoading = value"
            @isCanada="isCanadaSelected($event)" />
          <div class="columns is-multiline">
            <div class="column is-12">
              <PhoneInput ref="phoneComponent" :required="true" model="Mobile Number"
                :defaultValue="worker.mobileNumber as string" @formattedPhone="(phone) => (worker.mobileNumber = phone)" />
            </div>
          </div>
          <div class="step-navigation-buttons">
            <span></span>
            <b-button type="is-primary" @click="validateAndGoToStep(1)">
              {{ 'Next' }}
            </b-button>
          </div>
        </b-step-item>
        <b-step-item step="2" label="Preferences" :visible="!isLogin" :clickable="false">
          <h1 class="title has-text-centered">Preferences</h1>
          <div class="columns is-multiline">
            <div class="column is-12">
              <b-field :label="'Availability'">
                <div class="columns is-multiline">
                  <div class="column is-6 is-4-desktop" v-for="item in availabilities"
                    v-bind:key="item.id">
                    <b-checkbox v-model="worker.availabilities" :native-value="item">
                      {{ item.value }}
                    </b-checkbox>
                  </div>
                </div>
              </b-field>
            </div>
            <div class="column is-12">
              <b-field :label="'Available Time'">
                <div class="columns is-multiline">
                  <div class="column is-6" v-for="t in availabilityTimes"
                    v-bind:key="t.id">
                    <b-checkbox v-model="worker.availabilityTimes" :native-value="t">
                      {{ t.value }}
                    </b-checkbox>
                  </div>
                </div>
              </b-field>
            </div>
            <div class="column is-12">
              <b-field :label="'Available days'">
                <div class="columns is-multiline">
                  <div class="column is-6 is-3-desktop">
                    <b-checkbox v-model="allDaysSelected" @update:modelValue="changeDaysSelected">
                      All Days
                    </b-checkbox>
                  </div>
                  <div class="column is-6 is-3-desktop" v-for="day in days" v-bind:key="day.id">
                    <b-checkbox v-model="worker.availabilityDays" :native-value="day" @update:modelValue="changeAllDays">
                      {{ day.value }}
                    </b-checkbox>
                  </div>
                </div>
              </b-field>
            </div>
            <div class="column is-6">
              <b-field :label="'Can you Lift up to'">
                <b-select v-model="worker.lift" placeholder="Select option" expanded>
                  <option v-for="item in lifts" :value="item" v-bind:key="item.id">
                    {{ item.value }}
                  </option>
                </b-select>
              </b-field>
            </div>
            <div class="column is-6">
              <b-field :label="'Do you have your own vehicle?'">
                <b-switch v-model="worker.hasVehicle" :true-value="true" :false-value="false">
                  {{ worker.hasVehicle ? "Yes" : "No" }}
                </b-switch>
              </b-field>
            </div>
            <div class="column is-12">
              <b-field :label="'Languages'">
                <b-taginput v-model="worker.languages" autocomplete :data="filteredLanguages" open-on-focus
                  field="value" icon="label" placeholder="Select Languages" @typing="getFilteredLanguages">
                </b-taginput>
              </b-field>
            </div>
            <div class="column is-12">
              <b-field :label="'Skills'">
                <b-taginput v-model="worker.skills" autocomplete :data="filteredSkills" open-on-focus field="skill"
                  icon="label" placeholder="Select or Add Skills" :maxlength="20" allow-new @typing="getFilteredSkills"
                  :create-tag="addSkill">
                </b-taginput>
              </b-field>
              <span v-show="errors.workerSkills" class="help is-danger no-margin">
                {{ errors.workerSkills || '' }}
              </span>
            </div>
          </div>
          <div class="step-navigation-buttons">
            <b-button @click="goToPreviousStep()">
              {{ 'Previous' }}
            </b-button>
            <b-button type="is-primary" @click="validateAndGoToStep(2)">
              {{ 'Next' }}
            </b-button>
          </div>
        </b-step-item>
        <b-step-item :step="isLogin ? 2 : 3" label="Documents" :clickable="false">
          <h1 class="title has-text-centered">Documents</h1>
          <div class="columns is-multiline">
            <div class="column is-12">
              <div class="columns is-multiline document-section-header">
                <div class="column is-6">
                  <label class="fz1 has-text-weight-semibold section-label">Documents <span class="has-text-danger">*</span></label>
                </div>
                <div class="column is-6 upload-button-container">
                  <b-field class="file is-primary upload-field" :class="{
                    'has-name': !!selectedDocumentFile,
                    'upload-disabled': worker.identificationType1File && worker.identificationType2File
                  }">
                    <b-upload v-model="selectedDocumentFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
                      @update:modelValue="handleIdentificationUpload"
                      :disabled="worker.identificationType1File && worker.identificationType2File" :loading="isLoading"
                      class="file-label" rounded>
                      <span class="file-cta">
                        <b-icon class="file-icon" icon="upload"></b-icon>
                        <span class="file-label">
                          {{ selectedDocumentFile ? selectedDocumentFile.name : "Add file" }}
                        </span>
                      </span>
                    </b-upload>
                  </b-field>
                </div>
              </div>
              <div class="container-files">
                <div class="" v-if="worker.identificationType1File">
                  <div class="document-card">
                    <div class="columns is-multiline document-card-header">
                      <div class="column is-10-mobile is-10 no-padding">
                        <div class="document-icon-title">
                          <b-icon icon="file-document" size="is-small" class="document-icon"></b-icon>
                          <h4 class="has-text-weight-semibold document-filename">
                            {{ filename(worker.identificationType1File.fileName) }}
                          </h4>
                        </div>
                      </div>
                      <div class="column is-2-mobile is-2 document-delete-container no-padding">
                        <b-tooltip label="Delete" type="is-dark" position="is-top" append-to-body>
                          <b-button type="is-danger" size="is-small" icon-left="delete" outlined
                            @click="deleteDocument(worker.identificationType1File)">
                          </b-button>
                        </b-tooltip>
                      </div>
                    </div>
                    <div class="columns is-multiline">
                      <div class="column is-6">
                        <b-field :type="errors.identificationType1 ? 'is-danger' : ''"
                          :message="errors.identificationType1 || ''">
                          <template #label>
                            Identification Type <span class="has-text-danger">*</span>
                          </template>
                          <b-select v-model="identificationType1" name="identificationType1"
                            placeholder="Select identification type" expanded>
                            <option v-for="(type, index) in identificationTypes" :value="type"
                              :disabled="type === identificationType2"
                              v-bind:key="'identificationType1' + index">
                              {{ type.value }}
                            </option>
                          </b-select>
                        </b-field>
                      </div>
                      <div class="column is-6">
                        <b-field :type="errors.identificationNumber1 ? 'is-danger' : ''"
                          :message="errors.identificationNumber1 || ''">
                          <template #label>
                            Identification Number <span class="has-text-danger">*</span>
                          </template>
                          <b-input type="text" :placeholder="'Identification Number'"
                            v-model="identificationNumber1" name="identificationNumber1" />
                        </b-field>
                      </div>
                    </div>
                  </div>
                </div>
                <div class="" v-if="worker.identificationType2File">
                  <div class="document-card">
                    <div class="columns is-multiline document-card-header">
                      <div class="column is-10-mobile is-10 no-padding">
                        <div class="document-icon-title">
                          <b-icon icon="file-document" size="is-small" class="document-icon"></b-icon>
                          <h4 class="has-text-weight-semibold document-filename">
                            {{ filename(worker.identificationType2File.fileName) }}
                          </h4>
                        </div>
                      </div>
                      <div class="column is-2-mobile is-2 document-delete-container no-padding">
                        <b-tooltip label="Delete" type="is-dark" position="is-top" append-to-body>
                          <b-button type="is-danger" size="is-small" icon-left="delete" outlined
                            @click="deleteDocument(worker.identificationType2File)">
                          </b-button>
                        </b-tooltip>
                      </div>
                    </div>
                    <div class="columns is-multiline">
                      <div class="column is-6">
                        <b-field :type="errors.identificationType2 ? 'is-danger' : ''"
                          :message="errors.identificationType2 || ''">
                          <template #label>
                            Identification Type <span class="has-text-danger">*</span>
                          </template>
                          <b-select v-model="identificationType2" name="identificationType2"
                            placeholder="Select identification type" expanded>
                            <option v-for="(type, index) in identificationTypes" :value="type"
                              :disabled="type === identificationType1"
                              v-bind:key="'identificationType2' + index">
                              {{ type.value }}
                            </option>
                          </b-select>
                        </b-field>
                      </div>
                      <div class="column is-6">
                        <b-field :type="errors.identificationNumber2 ? 'is-danger' : ''"
                          :message="errors.identificationNumber2 || ''">
                          <template #label>
                            Identification Number <span class="has-text-danger">*</span>
                          </template>
                          <b-input type="text" :placeholder="'Identification Number'"
                            v-model="identificationNumber2" name="identificationNumber2" />
                        </b-field>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
              <span v-show="documentsError" class="help is-danger">
                At least one identification document is required
              </span>
            </div>
            <div class="column is-12">
              <div class="columns is-multiline document-section-header">
                <div class="column is-6">
                  <label class="fz1 has-text-weight-semibold section-label">{{ "Licenses" }}</label>
                </div>
                <div class="column is-6 upload-button-container">
                  <b-field class="file is-primary upload-field" :class="{ 'has-name': !!selectedLicenseFile }">
                    <b-upload v-model="selectedLicenseFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
                      @update:modelValue="handleLicenseUpload" :loading="isLoading" class="file-label" rounded>
                      <span class="file-cta">
                        <b-icon class="file-icon" icon="upload"></b-icon>
                        <span class="file-label">
                          {{ selectedLicenseFile ? selectedLicenseFile.name : "Add file" }}
                        </span>
                      </span>
                    </b-upload>
                  </b-field>
                </div>
              </div>
              <div class="container-files">
                <div class="" v-for="(item, index) in worker.licenses"
                  v-bind:key="'licences' + index">
                  <div class="document-card">
                    <div class="columns is-multiline document-card-header">
                      <div class="column is-10-mobile is-10 no-padding">
                        <div class="document-icon-title">
                          <b-icon icon="certificate" size="is-small" class="document-icon"></b-icon>
                          <h4 class="has-text-weight-semibold document-filename">
                            {{ filename(item.license.fileName) }}
                          </h4>
                        </div>
                      </div>
                      <div class="column is-2-mobile is-2 document-delete-container no-padding">
                        <b-tooltip label="Delete" type="is-dark" position="is-top" append-to-body>
                          <b-button type="is-danger" size="is-small" icon-left="delete" outlined
                            @click="deleteLicense(index)">
                          </b-button>
                        </b-tooltip>
                      </div>
                    </div>
                    <div class="columns is-multiline">
                      <div class="column is-8">
                        <b-field :type="itemErrors['description' + index] ? 'is-danger' : ''"
                          :message="itemErrors['description' + index] || ''">
                          <template #label>
                            Description <span class="has-text-danger">*</span>
                          </template>
                          <b-input type="text" :placeholder="'Description'" v-model="item.license.description"
                            :name="'description' + index" />
                        </b-field>
                      </div>
                      <div class="column is-4">
                        <b-field :type="itemErrors['licenseExpires' + index] ? 'is-danger' : ''"
                          :message="itemErrors['licenseExpires' + index] || ''">
                          <template #label>
                            Expires In <span class="has-text-danger">*</span>
                          </template>
                          <b-datepicker v-model="item.expires" :name="'licenseExpires' + index">
                          </b-datepicker>
                        </b-field>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="column is-12">
              <div class="columns is-multiline document-section-header">
                <div class="column is-6">
                  <label class="fz1 has-text-weight-semibold section-label">{{ "Certificates" }}</label>
                </div>
                <div class="column is-6 upload-button-container">
                  <b-field class="file is-primary upload-field" :class="{ 'has-name': !!selectedCertificateFile }">
                    <b-upload v-model="selectedCertificateFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
                      @update:modelValue="handleCertificateUpload" :loading="isLoading" class="file-label" rounded>
                      <span class="file-cta">
                        <b-icon class="file-icon" icon="upload"></b-icon>
                        <span class="file-label">
                          {{ selectedCertificateFile ? selectedCertificateFile.name : "Add file" }}
                        </span>
                      </span>
                    </b-upload>
                  </b-field>
                </div>
              </div>
              <div class="container-files">
                <div class="" v-for="(item, index) in worker.certificates"
                  v-bind:key="'certificates' + index">
                  <div class="document-card">
                    <div class="columns is-multiline document-card-header">
                      <div class="column is-10-mobile is-10 no-padding">
                        <div class="document-icon-title">
                          <b-icon icon="card-account-details" size="is-small" class="document-icon"></b-icon>
                          <h4 class="has-text-weight-semibold document-filename">{{ filename(item.fileName) }}</h4>
                        </div>
                      </div>
                      <div class="column is-2-mobile is-2 document-delete-container no-padding">
                        <b-tooltip label="Delete" type="is-dark" position="is-top" append-to-body>
                          <b-button type="is-danger" size="is-small" icon-left="delete" outlined
                            @click="deleteCertificate(index)">
                          </b-button>
                        </b-tooltip>
                      </div>
                    </div>
                    <b-field :type="itemErrors['descriptioncer' + index] ? 'is-danger' : ''"
                      :message="itemErrors['descriptioncer' + index] || ''"
                      label="Description">
                      <b-input type="text" placeholder="Description" v-model="item.description"
                        :name="'descriptioncer' + index" />
                    </b-field>
                  </div>
                </div>
              </div>
            </div>
            <div class="column is-12">
              <div class="columns is-multiline document-section-header">
                <div class="column is-6">
                  <label class="fz1 has-text-weight-semibold section-label">{{ "Resume" }}</label>
                </div>
                <div class="column is-6 upload-button-container">
                  <b-field class="file is-primary upload-field" :class="{
                    'has-name': !!selectedResumeFile,
                    'upload-disabled': worker.resume
                  }">
                    <b-upload v-model="selectedResumeFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
                      @update:modelValue="handleResumeUpload" :disabled="worker.resume ? true : false" :loading="isLoading"
                      class="file-label" rounded>
                      <span class="file-cta">
                        <b-icon class="file-icon" icon="upload"></b-icon>
                        <span class="file-label">
                          {{ selectedResumeFile ? selectedResumeFile.name : "Add file" }}
                        </span>
                      </span>
                    </b-upload>
                  </b-field>
                </div>
              </div>
              <div class="container-files">
                <div class="" v-if="worker.resume">
                  <div class="document-card">
                    <div class="columns is-multiline document-card-header">
                      <div class="column is-10-mobile is-10 no-padding">
                        <div class="document-icon-title">
                          <b-icon icon="file-account" size="is-small" class="document-icon"></b-icon>
                          <h4 class="has-text-weight-semibold document-filename">
                            {{ filename(worker.resume.fileName) }}
                          </h4>
                        </div>
                      </div>
                      <div class="column is-2-mobile is-2 document-delete-container no-padding">
                        <b-tooltip label="Delete" type="is-dark" position="is-top" append-to-body>
                          <b-button type="is-danger" size="is-small" icon-left="delete" outlined
                            @click="deleteResume()">
                          </b-button>
                        </b-tooltip>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="column is-12" v-if="isCanada">
              <div class="columns is-multiline document-section-header">
                <div class="column is-6">
                  <div>
                    <label class="fz1 has-text-weight-semibold section-label block-label">WHMIS and Health and Safety Training</label>
                    <i class="fz-2">Complete the training following both links below and upload your certificates</i>
                  </div>
                </div>
                <div class="column is-6 upload-button-container">
                  <b-field class="file is-primary upload-field" :class="{ 'has-name': !!selectedOtherDocFile }">
                    <b-upload v-model="selectedOtherDocFile" accept=".pdf,.jpeg,.jpg,.png,.gif,.doc,.docx,.xls,.xlsx"
                      @update:modelValue="handleOtherDocumentUpload" :loading="isLoading" class="file-label" rounded>
                      <span class="file-cta">
                        <b-icon class="file-icon" icon="upload"></b-icon>
                        <span class="file-label">
                          {{ selectedOtherDocFile ? selectedOtherDocFile.name : "Add file" }}
                        </span>
                      </span>
                    </b-upload>
                  </b-field>
                </div>
              </div>
              <div class="canada-links-container">
                <p class="canada-link">
                  <a href="https://aixsafety.com/wp-content/uploads/articulate_uploads/WHS-Apr2025Aix/story.html"
                    target="_blank" class="color-primary has-text-weight-semibold">WHIMS Training</a>
                </p>
                <p>
                  <a href="https://www.labour.gov.on.ca/english/hs/elearn/worker/foursteps.php" target="_blank"
                    class="color-primary has-text-weight-semibold">HS BOOKLET</a>
                </p>
              </div>
              <div class="container-files">
                <div class="" v-for="(item, index) in worker.otherDocuments"
                  v-bind:key="'otherDocument' + index">
                  <div class="document-card">
                    <div class="columns is-multiline document-card-header">
                      <div class="column is-10-mobile is-10 no-padding">
                        <div class="document-icon-title">
                          <b-icon icon="folder-open" size="is-small" class="document-icon"></b-icon>
                          <h4 class="has-text-weight-semibold document-filename">{{ filename(item.fileName) }}</h4>
                        </div>
                      </div>
                      <div class="column is-2-mobile is-2 document-delete-container no-padding">
                        <b-tooltip label="Delete" type="is-dark" position="is-top" append-to-body>
                          <b-button type="is-danger" size="is-small" icon-left="delete" outlined
                            @click="deleteOtherDocument(index)">
                          </b-button>
                        </b-tooltip>
                      </div>
                    </div>
                    <b-field :type="itemErrors['descriptionOther' + index] ? 'is-danger' : ''"
                      :message="itemErrors['descriptionOther' + index] || ''"
                      label="Description">
                      <b-input type="text" placeholder="Description" v-model="item.description"
                        :name="'descriptionOther' + index" />
                    </b-field>
                  </div>
                </div>
              </div>
            </div>
          </div>
          <div class="step-navigation-buttons">
            <b-button @click="goToPreviousStep()">
              {{ 'Previous' }}
            </b-button>
            <b-button type="is-primary" @click="validateAndGoToStep(3)">
              {{ 'Next' }}
            </b-button>
          </div>
        </b-step-item>
        <b-step-item :step="isLogin ? 3 : 4" label="Account" :clickable="false">
          <h1 class="title has-text-centered">Account</h1>
          <div class="columns is-multiline">
            <div class="column is-12">
              <b-field :type="errors.email ? 'is-danger' : ''"
                :message="errors.email || ''">
                <template #label>
                  {{ 'Email' }} <span class="has-text-danger">*</span>
                </template>
                <b-input type="email" v-model="email" name="email"
                  :class="{ 'is-danger': !!errors.email }" />
              </b-field>
            </div>
            <div class="column is-6">
              <b-field :type="errors.password ? 'is-danger' : ''"
                :message="errors.password || ''">
                <template #label>
                  {{ 'Password' }} <span class="has-text-danger">*</span>
                </template>
                <b-input type="password" v-model="password" name="password" />
              </b-field>
            </div>
            <div class="column is-6">
              <b-field :type="errors.confirmPassword ? 'is-danger' : ''"
                :message="errors.confirmPassword || ''">
                <template #label>
                  {{ 'Confirm Password' }} <span class="has-text-danger">*</span>
                </template>
                <b-input type="password" v-model="confirmPassword" name="confirmPassword" />
              </b-field>
            </div>
            <div class="column is-12" v-if="!isLogin">
              <b-field>
                <b-checkbox v-model="agreeTermsAndConditions" name="agree terms">
                  {{ "I agree Sigook™" }}
                  <router-link to="/terms-and-conditions" target="_blank">
                    <u class="color-primary">{{ "Terms and Conditions" }}</u>
                  </router-link>
                  &
                  <router-link to="/privacy-policy" target="_blank">
                    <u class="color-primary">{{ "Privacy Policy" }}.</u>
                  </router-link>
                </b-checkbox>
              </b-field>
              <span v-show="errors.agreeTermsAndConditions" class="help is-danger no-margin">
                {{ errors.agreeTermsAndConditions || '' }}
              </span>
            </div>
            <div class="column is-12">
              <div class="step-navigation-buttons">
                <b-button @click="goToPreviousStep()">
                  {{ 'Previous' }}
                </b-button>
                <b-button type="is-primary" native-type="submit">
                  {{ "Register" }}
                </b-button>
              </div>
            </div>
          </div>
        </b-step-item>
      </b-steps>
    </form>
  </div>
</template>

<script setup lang="ts">
import { reactive, ref, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
import { useForm, useField } from 'vee-validate';
import * as yup from 'yup';
import { useAppStore } from '@/stores/app';
import { useSecurityStore } from '@/stores/security';
import { showAlertError, showAlertSuccess } from '@/utils/toast';
import dayjs from 'dayjs';
import { registerWorker } from '@/api/workerApi';
import { useCreateWorker } from '@/composables/useCreateWorker';
import { usePubSub } from '@/composables/usePubSub';
import { filename } from '@/utils/filters';
import { createMultipartFormData } from '@/utils/buildWorkerFormData';
import UploadImage from '../../components/PreviewImage.vue';
import AddressComponent from '../../components/Address.vue';
import PhoneInput from '../../components/PhoneInput.vue';

const router = useRouter();
const appStore = useAppStore();
const securityStore = useSecurityStore();
const createWorker = useCreateWorker();
const pubSub = usePubSub();

const {
  skills,
  filteredSkills,
  filteredLanguages,
  genders,
  identificationTypes,
  availabilities,
  availabilityTimes,
  days,
  lifts,
  worker,
  allDaysSelected,
  loadCatalogs,
  changeDaysSelected,
  changeAllDays,
  getFilteredSkills,
  getFilteredLanguages,
} = createWorker;

const isLogin = computed(() => !!securityStore.user);
const hasSecondId = computed(() => !!worker.identificationType2File);

const validationSchema = computed(() => {
  const shape: Record<string, any> = {
    firstName: yup.string().required('Name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
    lastName: yup.string().required('Last name is required').min(2, 'Min 2 characters').max(20, 'Max 20 characters'),
    birthDay: yup.mixed().required('Date of birth is required'),
    gender: yup.mixed().required('Gender is required'),
    identificationType1: yup.mixed().required('Identification type is required'),
    identificationNumber1: yup.string().required('Identification number is required').min(5, 'Min 5 characters').max(15, 'Max 15 characters'),
    email: yup.string().required('Email is required').email('Invalid email').min(6).max(50),
    password: yup.string().required('Password is required').min(6, 'Min 6 characters').max(100, 'Max 100 characters'),
    confirmPassword: yup
      .string()
      .required('Confirm password is required')
      .oneOf([yup.ref('password')], 'Passwords must match'),
  };
  if (hasSecondId.value) {
    shape.identificationType2 = yup.mixed().required('Identification type is required');
    shape.identificationNumber2 = yup.string().required('Identification number is required').min(5, 'Min 5 characters').max(15, 'Max 15 characters');
  }
  if (!isLogin.value) {
    shape.agreeTermsAndConditions = yup
      .boolean()
      .oneOf([true], 'You must accept the Terms & Conditions to continue');
  }
  return yup.object(shape);
});

const { errors: formErrors, setFieldValue, validateField } = useForm({
  validationSchema,
  initialValues: {
    firstName: '',
    lastName: '',
    birthDay: null,
    gender: null,
    identificationType1: null,
    identificationNumber1: '',
    identificationType2: null,
    identificationNumber2: '',
    email: '',
    password: '',
    confirmPassword: '',
    agreeTermsAndConditions: false,
  },
});

const { value: firstName } = useField<string>('firstName');
const { value: lastName } = useField<string>('lastName');
const { value: birthDay } = useField<any>('birthDay');
const { value: gender } = useField<any>('gender');
const { value: identificationType1 } = useField<any>('identificationType1');
const { value: identificationNumber1 } = useField<string>('identificationNumber1');
const { value: identificationType2 } = useField<any>('identificationType2');
const { value: identificationNumber2 } = useField<string>('identificationNumber2');
const { value: email } = useField<string>('email');
const { value: password } = useField<string>('password');
const { value: confirmPassword } = useField<string>('confirmPassword');
const { value: agreeTermsAndConditions } = useField<boolean>('agreeTermsAndConditions');

const interacted = reactive<Record<string, boolean>>({});
watch(firstName, () => { interacted.firstName = true; });
watch(lastName, () => { interacted.lastName = true; });
watch(birthDay, () => { interacted.birthDay = true; });
watch(gender, () => { interacted.gender = true; });
watch(identificationType1, () => { interacted.identificationType1 = true; });
watch(identificationNumber1, () => { interacted.identificationNumber1 = true; });
watch(identificationType2, () => { interacted.identificationType2 = true; });
watch(identificationNumber2, () => { interacted.identificationNumber2 = true; });
watch(email, () => { interacted.email = true; });
watch(password, () => { interacted.password = true; });
watch(confirmPassword, () => { interacted.confirmPassword = true; });
watch(agreeTermsAndConditions, () => { interacted.agreeTermsAndConditions = true; });

const errors = computed(() => {
  const out: Record<string, string> = {};
  for (const key of Object.keys(formErrors.value)) {
    out[key] = interacted[key] ? (formErrors.value[key] || '') : '';
  }
  return out;
});

function markInteracted(fields: string[]) {
  for (const f of fields) interacted[f] = true;
}

const itemErrors = reactive<Record<string, string>>({});

const activeStep = ref(0);
const disableStartDate = ref<any>(null);
const isLoading = ref(true);
const disabledDates = ref<any>(null);
const isCanada = ref(false);
const selectedDocumentFile = ref<File | null>(null);
const selectedLicenseFile = ref<File | null>(null);
const selectedCertificateFile = ref<File | null>(null);
const selectedResumeFile = ref<File | null>(null);
const selectedOtherDocFile = ref<File | null>(null);
const documentsError = ref<boolean | null>(null);
const fileObjects = reactive<any>({
  profileImage: null,
  identificationType1: null,
  identificationType2: null,
  licenses: [],
  certificates: [],
  resume: null,
  otherDocuments: [],
});

const addressComponent = ref<any>(null);
const phoneComponent = ref<any>(null);

async function registerWorkerFn() {
  isLoading.value = true;
  worker.firstName = firstName.value;
  worker.lastName = lastName.value;
  worker.birthDay = birthDay.value;
  worker.identificationType1 = identificationType1.value;
  worker.identificationNumber1 = identificationNumber1.value;
  worker.identificationType2 = identificationType2.value;
  worker.identificationNumber2 = identificationNumber2.value;
  worker.email = email.value;
  worker.password = password.value;
  worker.confirmPassword = confirmPassword.value;
  worker.agreeTermsAndConditions = agreeTermsAndConditions.value;
  worker.gender = { id: gender.value };

  try {
    const formData = await createMultipartFormData(worker, fileObjects);
    const id = await registerWorker(formData);
    isLoading.value = false;
    showAlertSuccess('Your account has been created');
    const route = isLogin.value ? `/recruiting/workers/${id}` : '/home';
    router.push(route);
  } catch (error: unknown) {
    isLoading.value = false;
    showAlertError((error as { data?: unknown }).data);
  }
}

async function validateForm() {
  const fields = ['email', 'password', 'confirmPassword'];
  if (!isLogin.value) fields.push('agreeTermsAndConditions');
  markInteracted(fields);
  const results = await Promise.all(fields.map((f) => validateField(f as Parameters<typeof validateField>[0])));
  const allValid = results.every((r: any) => r.valid);
  if (allValid) {
    registerWorkerFn();
  } else {
    showAlertError('Please make sure all required fields are filled out correctly');
  }
}

function goToPreviousStep() {
  if (activeStep.value > 0) {
    activeStep.value--;
    if (isLogin.value && activeStep.value === 1) {
      activeStep.value--;
    }
  }
}

async function validateStep1() {
  const fields = ['firstName', 'lastName', 'birthDay', 'gender'];
  markInteracted(fields);
  const results = await Promise.all(fields.map((f) => validateField(f as Parameters<typeof validateField>[0])));
  const fieldsValid = results.every((r: any) => r.valid);
  const addressValid = await addressComponent.value.validateAddress();
  const phoneValid = await phoneComponent.value.validatePhone();
  return fieldsValid && addressValid && phoneValid;
}

async function validateStep3() {
  documentsError.value = !worker.identificationType1File;
  if (documentsError.value) return false;

  const fields = ['identificationType1', 'identificationNumber1'];
  if (worker.identificationType2File) {
    fields.push('identificationType2', 'identificationNumber2');
  }
  markInteracted(fields);
  const results = await Promise.all(fields.map((f) => validateField(f as Parameters<typeof validateField>[0])));
  let valid = results.every((r: any) => r.valid);

  const next: Record<string, string> = {};
  worker.licenses.forEach((item: any, i: number) => {
    const desc = item?.license?.description || '';
    if (!desc) {
      next['description' + i] = 'Description is required';
      valid = false;
    } else if (desc.length > 100) {
      next['description' + i] = 'Max 100 characters';
      valid = false;
    } else if (!/^[-_ a-zA-Z0-9]+$/.test(desc)) {
      next['description' + i] = 'Only letters, numbers, spaces and -_';
      valid = false;
    }
    if (!item.expires) {
      next['licenseExpires' + i] = 'Expiration date is required';
      valid = false;
    }
  });
  worker.certificates.forEach((item: any, i: number) => {
    const desc = item?.description || '';
    if (!desc) {
      next['descriptioncer' + i] = 'Description is required';
      valid = false;
    } else if (desc.length > 100) {
      next['descriptioncer' + i] = 'Max 100 characters';
      valid = false;
    } else if (!/^[-_ a-zA-Z0-9]+$/.test(desc)) {
      next['descriptioncer' + i] = 'Only letters, numbers, spaces and -_';
      valid = false;
    }
  });
  worker.otherDocuments.forEach((item: any, i: number) => {
    const desc = item?.description || '';
    if (!desc) {
      next['descriptionOther' + i] = 'Description is required';
      valid = false;
    } else if (desc.length > 100) {
      next['descriptionOther' + i] = 'Max 100 characters';
      valid = false;
    } else if (!/^[-_ a-zA-Z0-9]+$/.test(desc)) {
      next['descriptionOther' + i] = 'Only letters, numbers, spaces and -_';
      valid = false;
    }
  });
  Object.keys(itemErrors).forEach((k) => delete itemErrors[k]);
  Object.assign(itemErrors, next);
  return valid;
}

async function validateAndGoToStep(currentStep: number) {
  let valid = false;
  if (currentStep === 1) {
    valid = await validateStep1();
  } else if (currentStep === 2) {
    valid = true;
  } else if (currentStep === 3) {
    valid = await validateStep3();
  }
  if (valid) {
    activeStep.value++;
    if (isLogin.value && activeStep.value === 1) {
      activeStep.value++;
    }
  } else {
    showAlertError('Please make sure all required fields are filled out correctly');
  }
}

function validateDocumentFile(file: File | null, maxSizeKB = 15500): boolean {
  if (!file) return false;
  if (file.size / 1024 > maxSizeKB) {
    showAlertError('File exceeds 15MB limit');
    return false;
  }
  return true;
}

function addDocument(file: File) {
  if (!worker.identificationType1File) {
    fileObjects.identificationType1 = file;
    worker.identificationType1File = { fileName: file.name, description: '' };
    setFieldValue('identificationType1', null);
    setFieldValue('identificationNumber1', '');
  } else {
    fileObjects.identificationType2 = file;
    worker.identificationType2File = { fileName: file.name, description: '' };
    setFieldValue('identificationType2', null);
    setFieldValue('identificationNumber2', '');
  }
  documentsError.value = false;
}

function handleIdentificationUpload(file: File | null) {
  if (!file || !validateDocumentFile(file)) return;
  addDocument(file);
  selectedDocumentFile.value = null;
}

function addLicense(file: File) {
  fileObjects.licenses.push(file);
  worker.licenses.push({ license: { fileName: file.name, description: '' } });
}

function handleLicenseUpload(file: File | null) {
  if (!file || !validateDocumentFile(file)) return;
  addLicense(file);
  selectedLicenseFile.value = null;
}

function addCertificate(file: File) {
  fileObjects.certificates.push(file);
  worker.certificates.push({ fileName: file.name, description: '' });
}

function handleCertificateUpload(file: File | null) {
  if (!file || !validateDocumentFile(file)) return;
  addCertificate(file);
  selectedCertificateFile.value = null;
}

function addResume(file: File) {
  fileObjects.resume = file;
  worker.resume = { fileName: file.name };
}

function handleResumeUpload(file: File | null) {
  if (!file || !validateDocumentFile(file)) return;
  addResume(file);
  selectedResumeFile.value = null;
}

function addOtherDocument(file: File) {
  fileObjects.otherDocuments.push(file);
  worker.otherDocuments.push({ fileName: file.name, description: '' });
}

function handleOtherDocumentUpload(file: File | null) {
  if (!file || !validateDocumentFile(file)) return;
  addOtherDocument(file);
  selectedOtherDocFile.value = null;
}

function deleteDocument(file: any) {
  if (!worker.identificationType1File) return;
  const isFile1 = worker.identificationType1File.fileName === file.fileName;
  if (isFile1 && worker.identificationType2File) {
    worker.identificationType1File = { ...worker.identificationType2File };
    fileObjects.identificationType1 = fileObjects.identificationType2;
    setFieldValue('identificationType1', identificationType2.value);
    setFieldValue('identificationNumber1', identificationNumber2.value);
    fileObjects.identificationType2 = null;
    worker.identificationType2File = null;
    setFieldValue('identificationType2', null);
    setFieldValue('identificationNumber2', '');
  } else if (isFile1) {
    fileObjects.identificationType1 = null;
    worker.identificationType1File = null;
    setFieldValue('identificationType1', null);
    setFieldValue('identificationNumber1', '');
  } else {
    fileObjects.identificationType2 = null;
    worker.identificationType2File = null;
    setFieldValue('identificationType2', null);
    setFieldValue('identificationNumber2', '');
  }
}

function deleteLicense(index: number) {
  fileObjects.licenses.splice(index, 1);
  worker.licenses.splice(index, 1);
}

function deleteCertificate(index: number) {
  fileObjects.certificates.splice(index, 1);
  worker.certificates.splice(index, 1);
}

function deleteResume() {
  fileObjects.resume = null;
  worker.resume = null;
}

function deleteOtherDocument(index: number) {
  fileObjects.otherDocuments.splice(index, 1);
  worker.otherDocuments.splice(index, 1);
}

function saveImage(image: any) {
  fileObjects.profileImage = image;
  if (!worker.profileImage) {
    worker.profileImage = { fileName: image.name };
  } else {
    (worker.profileImage as { fileName: string }).fileName = image.name;
  }
}

function addSkill(skill: any) {
  if (!skill.skill) {
    skill = { skill };
    skills.value.push(skill);
  }
  return skill;
}

function isCanadaSelected(value: boolean) {
  isCanada.value = value;
  isLoading.value = true;
  if (value === false && worker.otherDocuments.length > 0) {
    fileObjects.otherDocuments = [];
    worker.otherDocuments = [];
  }
  isLoading.value = false;
}

(async () => {
  await loadCatalogs();
  isLoading.value = false;
  appStore.getCurrentDate().then((response: any) => {
    disableStartDate.value = response;
    disabledDates.value = dayjs(response).subtract(18, 'years').toDate();
  });
})();
</script>

<style lang="scss" scoped>
.form-md {
  width: 80%;
  min-width: 800px;
  margin: 30px auto;

  @media (max-width: 900px) {
    width: 100%;
    min-width: 0;
    margin: 0;
  }
}

.container-files {
  padding: 10px 0;
}

.document-section-header {
  align-items: center;
  margin-bottom: 15px;
}

.section-label {
  margin-bottom: 0;
}

.upload-button-container {
  text-align: right;
}

.upload-field {
  margin-bottom: 0;
}

.document-card {
  background: #fafafa;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  padding: 20px;
  margin-bottom: 15px;
}

.document-card-header {
  align-items: center;
  margin-bottom: 15px;
}

.document-icon-title {
  display: flex;
  align-items: center;
}

.document-icon {
  margin-right: 10px;
  color: #7957d5;
}

.document-filename {
  margin: 0;
  color: #363636;
}

.document-delete-container {
  text-align: right;
}

.canada-links-container {
  margin-bottom: 15px;
}

.canada-link {
  margin-bottom: 5px;
}

.upload-image-spacing {
  margin-top: 23px;
  margin-bottom: 0;
}

.no-padding {
  padding: 0;
}

.block-label {
  display: block;
}

.upload-disabled {
  opacity: 0.5;
  cursor: not-allowed;
  pointer-events: none;
}

.step-navigation-buttons {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 20px;
  padding-top: 20px;
  border-top: 1px solid #e0e0e0;
}
</style>
