import {
	RECIPE_PHOTO_UPLOAD_SUCCESS,
	RECIPE_PHOTO_UPLOAD_FAIL,
	USER_PHOTO_UPLOAD_FAIL,
	USER_PHOTO_DELETION_ERROR
} from '../actions/types';

const initialState = {
	userPhotos: [],
	postPhotos: [],
	loading: true,
	error: {}
};

export default function(state = initialState, action) {
	const { type, payload } = action;

	switch (type) {
		case RECIPE_PHOTO_UPLOAD_SUCCESS:
			return {
				...state,
				loading: false,
				postPhotos: [ payload, ...state.postPhotos ],
				userPhotos: []
			};
		case USER_PHOTO_UPLOAD_FAIL:
		case USER_PHOTO_DELETION_ERROR:
		case RECIPE_PHOTO_UPLOAD_FAIL:
			return {
				...state,
				error: payload,
				loading: false
			};
		default:
			return state;
	}
}
